using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GameEndScripting : MonoBehaviour {

    [SerializeField]
    protected GameObject playerEntryPrefab;

    [SerializeField]
    protected float gameEndTime;

    [AutoLink(childPath = "WitchStats")]
    [SerializeField]
    public Transform witchesStats;

    [AutoLink(childPath = "AlienStats")]
    [SerializeField]
    public Transform aliensStats;

    public int leftScore {get; set;}
    public int rightScore { get; set; }

    public Dictionary<Transform, int> playerScores { get; set; }

    Transform canvas;

	// Use this for initialization
	void Awake () {
        GetComponent<AudioSource>().Play();
        canvas = GameObject.FindGameObjectWithTag(Tags.canvas).GetComponentInParent<Canvas>().transform;
	}

    void Start()
    {
        Camera.main.gameObject.AddComponent<BlitGreyscale>().time = gameEndTime;

        Observers.Post(new GameEndMessage(this, gameEndTime)); //sends this object around, elements add their data to this object

        Observers.Clear(GameEndMessage.classMessageType, GoalScoredMessage.classMessageType);

        //Callback.FireAndForget(() => { Application.LoadLevel(Tags.Scenes.select); Destroy(this); }, gameEndTime, this, mode: Callback.Mode.REALTIME);

        int maxScore = Mathf.Max(leftScore, rightScore);

        foreach (KeyValuePair<Transform, int> player in playerScores)
        {
            Side side = player.Key.GetComponent<Stats>().side;
            Transform instantiatedEntry = Instantiate(playerEntryPrefab).transform;
            instantiatedEntry.SetParent(side == Side.LEFT ? witchesStats : aliensStats, false);
            GameEndPlayerEntry gameEndEntry = instantiatedEntry.GetComponentInChildren<GameEndPlayerEntry>();
            gameEndEntry.Init(player.Value, maxScore, player.Key.GetComponentInChildren<AbstractPlayerVisuals>());
        }

        /*
        if (leftScore < rightScore)
            Instantiate(witchesVictoryPrefab).transform.SetParent(canvas, false);
        else if (leftScore > rightScore)
            Instantiate(aliensVictoryPrefab).transform.SetParent(canvas, false);
        */
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