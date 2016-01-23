using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Blit))]
public class GoalScreenTearing : MonoBehaviour, IObserver<Message> {

    [SerializeField]
    protected float time;

    [SerializeField]
    protected float initialMagnitude;

    Blit blit;

    void Awake()
    {
        blit = GetComponent<Blit>();
        Observers.Subscribe(this, GoalScoredMessage.classMessageType);
    }


    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                Callback.DoLerp((float l) => blit.intensity = initialMagnitude * l * l, time, this, reverse : true);
                break;
        }
    }
}
