using UnityEngine;
using System.Collections;

public class ResetScripting : MonoBehaviour, IObservable<ResetMessage> {

    Observable<ResetMessage> resetMessageObservable = new Observable<ResetMessage>(); public Observable<ResetMessage> Observable(IObservable<ResetMessage> self) { return resetMessageObservable; }
    InputToAction action;
    VisualAnimate vfx;
	// Use this for initialization
	void Awake () {
        action = GetComponent<InputToAction>();
        vfx = GetComponent<VisualAnimate>();
	}

    public void Reset(Vector2 newPos, float disabledTime)
    {
        resetMessageObservable.Post(new ResetMessage(disabledTime));
        action.DisableAbilities(disabledTime);
        transform.position = newPos;
        vfx.DoFX();
        action.DisableMovement(disabledTime);
    }
}

public class ResetMessage
{
    public readonly float timeTillActive;
    public ResetMessage(float timeTillActive)
    {
        this.timeTillActive = timeTillActive;
    }
}