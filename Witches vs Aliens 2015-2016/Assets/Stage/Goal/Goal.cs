using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class Goal : MonoBehaviour {
    [SerializeField]
    protected Side mySide;
    public Side side { get { return mySide; } }

    ParticleSystem vfx;
    AudioSource sfx;

    [SerializeField]
    protected AudioClip goalSound;
    [SerializeField]
    protected AudioClip crySound;
    void Start()
    {
        vfx = GetComponent<ParticleSystem>();
        sfx = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other) {
        if (!other.collider.CompareTag(Tags.puck))
            return;
        
        other.gameObject.GetComponent<PuckFX>().Hide();
        PlayGoalFX();
        Observers.Post(new GoalScoredMessage(mySide));
	}

    public void PlayGoalFX()
    {
        Debug.Log("GOOOOOOOOOOOOOOOOOOOOOOAL!");
        vfx.Play();
        sfx.spread = 0;
        sfx.clip = goalSound;
        sfx.Play();
        Callback.FireAndForget(() => { sfx.spread = 360; sfx.clip = crySound; sfx.Play(); }, sfx.clip.length / 2, this);
        ScreenShake.RandomShake(this, 0.1f, 0.25f);
    }
}

public enum Side
{
    LEFT,
    RIGHT
}

public class GoalScoredMessage : Message
{
    public readonly Side side;
    public const string classMessageType = "GoalScoredMessage";
    public GoalScoredMessage(Side side) : base(classMessageType)
    {
        this.side = side;
    }
}

