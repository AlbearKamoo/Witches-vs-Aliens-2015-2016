using UnityEngine;
using System.Collections;

public class ResetScripting : MonoBehaviour, IObservable<ResetMessage> {

    Observable<ResetMessage> resetMessageObservable = new Observable<ResetMessage>(); public Observable<ResetMessage> Observable() { return resetMessageObservable; }
    InputToAction action;
    MovementAbility movementAbility;
    GenericAbility genericAbility;
	// Use this for initialization
	void Awake () {
        action = GetComponent<InputToAction>();
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
        action.DisableMovement(disabledTime);
    }
}

public class ResetMessage { }