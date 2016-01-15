using UnityEngine;
using System.Collections;

//this particular script has to be placed on the top level of a player via AddComponent, b/c of OnCollisionEnter()

public class Contagion : MonoBehaviour {

    Countdown startCountdown;
    ContagionAbility origin;
    ContagionEffects effects;

    public bool active
    {
        get
        {
            return startCountdown.active;
        }
        set
        {
            this.enabled = value;
            if (value)
            {
                startCountdown.Start();
            }
            effects.active = value;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (active && other.collider.CompareTag(Tags.player))
        {
            origin.TryAddContagion(other.transform);
        }
    }

    public void Initialize(float duration, ContagionAbility origin, ContagionEffects effects)
    {
        startCountdown = Countdown.TimedCountdown(() => active = false, duration, this);
        this.origin = origin;
        this.effects = effects;
    }

}
