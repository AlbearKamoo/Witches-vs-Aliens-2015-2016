using UnityEngine;
using System.Collections;

public class GameEndScripting : MonoBehaviour {

    [SerializeField]
    protected float gameEndTime;

    public float leftScore;
    public float rightScore;

	// Use this for initialization
	void Awake () {
        Camera.main.gameObject.AddComponent<BlitGreyscale>().time = gameEndTime;
        Observers.Post(new GameEndMessage(this, gameEndTime));
        Observers.Clear(GameEndMessage.classMessageType, GoalScoredMessage.classMessageType);
        Pause.pause();
        Callback.FireAndForgetRealtime(() => { Application.LoadLevel(Tags.Scenes.select); Pause.unPause(); Destroy(this); }, gameEndTime, this);
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