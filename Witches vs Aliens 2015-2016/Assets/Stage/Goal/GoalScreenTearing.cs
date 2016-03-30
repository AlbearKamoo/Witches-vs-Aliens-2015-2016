using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Blit))]
public class GoalScreenTearing : MonoBehaviour, IObserver<Message> {

    [SerializeField]
    protected float time;

    [SerializeField]
    protected float initialMagnitude;

    public static GoalScreenTearing self;

    Blit blit;
    bool active = false;
    public bool Active {
        get { return active; }
    }

    void Awake()
    {
        self = this;
        blit = GetComponent<Blit>();
        Observers.Subscribe(this, GoalScoredMessage.classMessageType);
    }

    void OnDestroy()
    {
        Observers.Unsubscribe(this, GoalScoredMessage.classMessageType);
    }


    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                active = true;
                Callback.DoLerp((float l) => blit.intensity = initialMagnitude * l * l, time, this, reverse: true).FollowedBy(() => active = false, this);
                break;
        }
    }

    public void doTear(float time, float magnitude)
    {
        if (!active)
        {
            active = true;
            Callback.DoLerp((float l) => blit.intensity = magnitude * l, time, this, reverse: true).FollowedBy(() => active = false, this);
        }
    }
}
