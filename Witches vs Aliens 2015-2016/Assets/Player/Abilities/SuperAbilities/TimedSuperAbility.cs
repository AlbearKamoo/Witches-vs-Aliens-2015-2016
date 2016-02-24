using UnityEngine;
using System.Collections;

public class TimedSuperAbility : SuperAbility
{

    [SerializeField]
    protected float duration;

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        active = true;

        Callback.FireAndForget(() => active = false, duration, this);
    }
}
