using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReverseSuper : SuperAbility, IOpponentsAbility {

    [SerializeField]
    protected float duration;

    List<Transform> _opponents;
    public List<Transform> opponents { set { _opponents = value; } }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        active = true;
        Swap();

        Callback.FireAndForget(Swap, duration, this);
    }

    void Swap()
    {
        foreach (Transform opponent in _opponents)
        {
            AbstractPlayerInput input = opponent.GetComponent<AbstractPlayerInput>();
            string temp = input.bindings.horizontalMovementAxisName;
            input.bindings.horizontalMovementAxisName = input.bindings.verticalMovementAxisName;
            input.bindings.verticalMovementAxisName = temp;
        }
    }
}
