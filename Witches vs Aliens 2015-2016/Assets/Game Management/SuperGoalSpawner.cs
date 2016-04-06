using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.Audio;

//should be placed on the parent of all the supergoal spawn pairs
[RequireComponent(typeof(AudioSource))]
public class SuperGoalSpawner : MonoBehaviour, INetworkable {
    [SerializeField]
    protected GameObject SuperGoalPrefab;

    [SerializeField]
    protected Transform[] spawnPositions;

    [SerializeField]
    protected float spawnTime;
    [SerializeField]
    protected float spawnTimeVariance;
    [SerializeField]
    protected float superGoalDuration;

    [SerializeField]
    protected AudioClip WitchesSuperClip;
    [SerializeField]
    protected AudioClip AliensSuperClip;

    [SerializeField]
    protected AudioMixerGroup aliensSuperOutput;
    [SerializeField]
    protected AudioMixerGroup witchesSuperOutput;

    SuperGoal SuperGoal1;
    SuperGoal SuperGoal2;
    void SetGoalsActive(bool active) { SuperGoal1.active = active; SuperGoal2.active = active; puckFX.perSideEffectsActive = active; } //setting a supergoal to false will destroy it
    NetworkNode node;
    NetworkMode mode;
    AudioSource sfx;

    PuckFX puckFX;

	// Use this for initialization
	void Awake () {
        Assert.IsTrue(spawnPositions.Length != 0);
        sfx = GetComponent<AudioSource>();
        Callback.FireForUpdate(() => puckFX = GameObject.FindGameObjectWithTag(Tags.puck).GetComponent<PuckFX>(), this);
        //TODO: maybe move to start instead of using callback?
    }

    void Start()
    {
        node = NetworkNode.node;
        if (node == null) //if null, no networking, server controls it if there is networking
        {
            mode = NetworkMode.UNKNOWN;
            StartCoroutine(LocalGoalSpawning());
        }
        else if(node is Server)
        {
            mode = NetworkMode.LOCALSERVER;
            StartCoroutine(ServerGoalSpawning());
        }
        else if (node is Client)
        {
            mode = NetworkMode.REMOTECLIENT;
            node.Subscribe(this);
        }
	}

    void spawnSuperGoals(int spawnPointIndex)
    {
        SuperGoal1 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal2 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal1.mirror = SuperGoal2;
        SuperGoal2.mirror = SuperGoal1;
        SuperGoal1.Spawner = this;
        SuperGoal2.Spawner = this;

        SuperGoal1.transform.SetParent(spawnPositions[spawnPointIndex], false);
        SuperGoal2.transform.SetParent(spawnPositions[spawnPointIndex].Find("Mirror"), false);

        SetGoalsActive(true);

        Callback.FireAndForget(() => { SetGoalsActive(false); }, superGoalDuration, this);
    }

    public void OnSuperGoalScored(LastBumped bumped)
    {
        switch (mode)
        {
            case NetworkMode.LOCALSERVER:
                node.BinaryWriter.Write(PacketType.SUPERGOALSCORED);
                node.BinaryWriter.Write((byte)(bumped.lastBumpedPlayer.GetComponent<Stats>().playerID));
                node.Send(node.AllCostChannel);
                goto case NetworkMode.UNKNOWN;
            case NetworkMode.UNKNOWN:
                bumped.lastBumpedPlayer.GetComponentInChildren<SuperAbility>().ready = true;
                playFX(bumped.side);
                break;
            /*case NetworkMode.REMOTECLIENT:
                break;*/ //wait for server's verification
        }
        
    }

    void playFX(Side side)
    {
        switch (side)
        {
            case Side.LEFT:
                sfx.clip = WitchesSuperClip;
                sfx.outputAudioMixerGroup = witchesSuperOutput;
                break;
            case Side.RIGHT:
                sfx.clip = AliensSuperClip;
                sfx.outputAudioMixerGroup = aliensSuperOutput;
                break;
        }
        sfx.Play();
        SetGoalsActive(false);
    }

    IEnumerator LocalGoalSpawning()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(RandomLib.RandFloatRange(spawnTime, spawnTimeVariance));
            spawnSuperGoals(Random.Range(0, spawnPositions.Length));
        }
    }

    IEnumerator ServerGoalSpawning()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(RandomLib.RandFloatRange(spawnTime, spawnTimeVariance));
            node.BinaryWriter.Write(PacketType.SUPERGOALSPAWNING);
            byte spawnPointIndex = (byte)Random.Range(0, spawnPositions.Length);
            node.BinaryWriter.Write(spawnPointIndex);
            node.Send(node.AllCostChannel);
            spawnSuperGoals(spawnPointIndex);
        }
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.SUPERGOALSPAWNING, PacketType.SUPERGOALSCORED }; } }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.SUPERGOALSPAWNING:
                int spawnPointIndex = m.reader.ReadByte();
                spawnSuperGoals(spawnPointIndex);
                break;
            case PacketType.SUPERGOALSCORED:
                int playerID = m.reader.ReadByte();
                GameObject player = InputToAction.IDToInputToAction(playerID).gameObject;
                player.GetComponentInChildren<SuperAbility>().ready = true;
                playFX(player.GetComponent<Stats>().side);
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}
