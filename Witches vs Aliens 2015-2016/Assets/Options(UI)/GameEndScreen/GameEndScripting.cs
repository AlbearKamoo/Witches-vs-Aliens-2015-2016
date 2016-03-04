using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameEndScripting : MonoBehaviour {

    [SerializeField]
    protected float gameEndTime;

    public GameObject witchesVictoryPrefab;
    public GameObject aliensVictoryPrefab;

    public float leftScore {get; set;}
    public float rightScore { get; set; }

    Transform canvas;

	// Use this for initialization
	void Awake () {
        GetComponent<AudioSource>().Play();
        Debug.Log(".");
        canvas = GameObject.FindGameObjectWithTag(Tags.canvas).GetComponentInParent<Canvas>().transform;
	}

    void Start()
    {
        Camera.main.gameObject.AddComponent<BlitGreyscale>().time = gameEndTime;
        Observers.Post(new GameEndMessage(this, gameEndTime));
        Observers.Clear(GameEndMessage.classMessageType, GoalScoredMessage.classMessageType);
        Callback.FireAndForget(() => { Application.LoadLevel(Tags.Scenes.select); Pause.unPause(); Destroy(this); }, gameEndTime, this, mode: Callback.Mode.REALTIME);
        if (leftScore < rightScore)
            Instantiate(witchesVictoryPrefab).transform.SetParent(canvas, false);
        else if (leftScore > rightScore)
            Instantiate(aliensVictoryPrefab).transform.SetParent(canvas, false);

        Callback.FireForUpdate(() => Pause.pause(), this);
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