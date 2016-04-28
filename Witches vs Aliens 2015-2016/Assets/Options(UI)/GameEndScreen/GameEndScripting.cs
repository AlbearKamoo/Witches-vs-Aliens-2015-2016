using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class GameEndScripting : MonoBehaviour, INetworkable
{

    [SerializeField]
    protected GameObject playerEntryPrefab;

    [SerializeField]
    protected AudioClip alienWinClip;
    [SerializeField]
    protected AudioClip witchWinClip;

    [SerializeField]
    protected float gameEndTime;
    [SerializeField]
    protected Image highlightGradient;

    [AutoLink(childPath = "WitchStats")]
    [SerializeField]
    public Transform witchesStats;

    [AutoLink(childPath = "AlienStats")]
    [SerializeField]
    public Transform aliensStats;

    [SerializeField]
    public CanvasGroup continueTooltip;

    public int leftScore {get; set;}
    public int rightScore { get; set; }

    public Dictionary<Transform, int> playerScores { get; set; }

    List<AbstractPlayerInput> inputs = new List<AbstractPlayerInput>();
    Transform canvas;
    AudioSource audioSource;

	// Use this for initialization
	void Awake () {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        canvas = GameObject.FindGameObjectWithTag(Tags.canvas).GetComponentInParent<Canvas>().transform;

        Image fade = GetComponent<Image>();
        Callback.DoLerp((float l) => fade.color = fade.color.setAlphaFloat(l), gameEndTime, this, mode : Callback.Mode.REALTIME);
	}

    void Start()
    {
        if (NetworkNode.node != null)
        {
            NetworkNode.node.Subscribe(this);
        }

        Observers.Post(new GameEndMessage(this, gameEndTime)); //sends this object around, elements add their data to this object

        Observers.Clear(GameEndMessage.classMessageType, GoalScoredMessage.classMessageType);

        //Callback.FireAndForget(() => { Application.LoadLevel(Tags.Scenes.select); Destroy(this); }, gameEndTime, this, mode: Callback.Mode.REALTIME);

        Callback.FireForUpdate(() => Pause.pause(), this);

        Callback.FireAndForget(() => { Pause.unPause(); SpawnEndScreen(); }, gameEndTime, this, mode: Callback.Mode.REALTIME);

        /*
        if (leftScore < rightScore)
            Instantiate(witchesVictoryPrefab).transform.SetParent(canvas, false);
        else if (leftScore > rightScore)
            Instantiate(aliensVictoryPrefab).transform.SetParent(canvas, false);
        */
    }

    void SpawnEndScreen()
    {
        Debug.Log(Time.timeScale);

        if(leftScore < rightScore)
        {
            audioSource.clip = witchWinClip;
        }
        else
        {
            audioSource.clip = alienWinClip;
        }
        
        audioSource.Play();

        int maxScore = Mathf.Max(leftScore, rightScore, 1);

        foreach (KeyValuePair<Transform, int> player in playerScores)
        {
            Side side = player.Key.GetComponent<Stats>().side;
            Transform instantiatedEntry = Instantiate(playerEntryPrefab).transform;
            instantiatedEntry.SetParent(side == Side.LEFT ? witchesStats : aliensStats, false);
            GameEndPlayerEntry gameEndEntry = instantiatedEntry.GetComponentInChildren<GameEndPlayerEntry>();
            gameEndEntry.Init(player.Value, maxScore, player.Key.GetComponentInChildren<AbstractPlayerVisuals>());

            AbstractPlayerInput playerInput = player.Key.GetComponent<AbstractPlayerInput>();
            if(playerInput != null)
                inputs.Add(playerInput);
        }

        highlightGradient.color = Color.white;

        Callback.FireAndForget(() => Callback.DoLerp((float l) => continueTooltip.alpha = l, gameEndTime, this), gameEndTime, this);
        if (NetworkNode.node == null || NetworkNode.node is Server)
        {
            StartCoroutine(CheckSceneTransition());
        }
    }

    IEnumerator CheckSceneTransition()
    {
        while (true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                changeScene();
                break;
            }

            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].pressedAccept())
                {
                    changeScene();
                    break;
                }
            }
        }
    }

    void changeScene()
    {
        if (NetworkNode.node is Server)
        {
            NetworkNode.node.BinaryWriter.Write(PacketType.SCENEJUMP);
            NetworkNode.node.Send(NetworkNode.node.AllCostChannel);
        }
        Application.LoadLevel(Tags.Scenes.select);
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.SCENEJUMP }; } }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.SCENEJUMP:
                Assert.IsTrue(NetworkNode.node is Client);
                Application.LoadLevel(Tags.Scenes.select);
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}

public class GameEndMessage : Message
{
    public GameEndScripting endData;
    public float time;
    public const string classMessageType = "GameEndMessage";
    public GameEndMessage(GameEndScripting endData, float time) : base(classMessageType)
    {
        this.endData = endData;
        this.time = time;
    }
}