using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

//should be placed on the parent of all the supergoal spawn pairs

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
    SuperGoal SuperGoal1;
    SuperGoal SuperGoal2;
    NetworkNode node;

	// Use this for initialization
	void Awake () {
        Assert.IsTrue(spawnPositions.Length != 0);
        SuperGoal1 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal2 = Instantiate(SuperGoalPrefab).GetComponent<SuperGoal>();
        SuperGoal1.mirror = SuperGoal2;
        SuperGoal2.mirror = SuperGoal1;
    }

    void Start()
    {
        node = NetworkNode.node;
        if (node == null) //if null, no networking, server controls it if there is networking
        {
            StartCoroutine(LocalGoalSpawning());
        }
        else if(node is Server)
        {
            StartCoroutine(ServerGoalSpawning());
        }
        else if (node is Client)
        {
            node.Subscribe(this);
        }
	}

    void spawnSuperGoals(int spawnPointIndex)
    {
        SuperGoal1.transform.SetParent(spawnPositions[spawnPointIndex], false);
        SuperGoal2.transform.SetParent(spawnPositions[spawnPointIndex].Find("Mirror"), false);

        SuperGoal1.active = true;
        SuperGoal2.active = true;

        Callback.FireAndForget(() => { SuperGoal1.active = false; SuperGoal2.active = false; }, superGoalDuration, this);
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
            node.BinaryWriter.Write((byte)(PacketType.SUPERGOALSPAWNING));
            byte spawnPointIndex = (byte)Random.Range(0, spawnPositions.Length);
            node.BinaryWriter.Write(spawnPointIndex);
            node.Send(node.AllCostChannel);
            spawnSuperGoals(spawnPointIndex);
        }
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.SUPERGOALSPAWNING}; } }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.SUPERGOALSPAWNING:
                int spawnPointIndex = m.reader.ReadByte();
                spawnSuperGoals(spawnPointIndex);
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}
