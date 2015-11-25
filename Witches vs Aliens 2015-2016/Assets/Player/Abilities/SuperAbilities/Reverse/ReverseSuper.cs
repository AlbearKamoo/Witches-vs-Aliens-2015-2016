using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReverseSuper : SuperAbility, IOpponentsAbility {

    [SerializeField]
    protected GameObject stunVisualsPrefab;

    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float visualsLerpTime;

    List<Transform> _opponents;
    public List<Transform> opponents { set { _opponents = value; } }

    protected override void Start()
    {
        base.Start();
        ready = true; //for easy testing
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        Swap();

        foreach (Transform opponent in _opponents)
        {
            GameObject visuals = SimplePool.Spawn(stunVisualsPrefab);
            visuals.transform.SetParent(opponent, false);
            SpriteRenderer rend = visuals.GetComponent<SpriteRenderer>();
            Callback.DoLerp((float l) => rend.color.setAlphaFloat(l), visualsLerpTime, this);
            Callback.FireAndForget(() => Callback.DoLerp((float l) => rend.color.setAlphaFloat(l), visualsLerpTime, this, reverse: true), duration - visualsLerpTime, this).FollowedBy(() => SimplePool.Despawn(visuals), this);
        }

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
