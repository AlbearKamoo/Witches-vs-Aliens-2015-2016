using UnityEngine;
using System.Collections;

public class ResetScripting : MonoBehaviour, IObservable<ResetMessage> {

    Observable<ResetMessage> resetMessageObservable = new Observable<ResetMessage>(); public Observable<ResetMessage> Observable() { return resetMessageObservable; }
    InputToAction action;
    MovementAbility movementAbility;
    GenericAbility genericAbility;
    VisualAnimate vfx;
	// Use this for initialization
	void Awake () {
        action = GetComponent<InputToAction>();
        vfx = GetComponent<VisualAnimate>();
	}

    void Start() {
        movementAbility = GetComponentInChildren<MovementAbility>();
        genericAbility = GetComponentInChildren<GenericAbility>();
    }

    public void Reset(Vector2 newPos, float disabledTime)
    {
        resetMessageObservable.Post(new ResetMessage());
        movementAbility.active = false;
        genericAbility.active = false;
        transform.position = newPos;
        vfx.DoFX();
        action.DisableMovement(disabledTime);
    }
}

public class ResetMessage { }