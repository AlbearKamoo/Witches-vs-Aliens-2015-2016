using UnityEngine;
using System.Collections;

//this particular script has to be placed on the top level of a player via AddComponent, b/c of OnCollisionEnter()

public class Contagion : MonoBehaviour {

    IEnumerator startCountdown;
    ContagionAbility origin;
    ContagionEffects effects;
    InputToAction action;
    FloatStat massMod;

    float massNerf;
    float duration;

    public bool active
    {
        get
        {
            return startCountdown != null;
        }
        set
        {
            this.enabled = value;
            if (value)
            {
                if (startCountdown == null)
                {
                    startCountdown = Callback.Routines.FireAndForgetRoutine(() => {active = false; startCountdown = null;}, duration, this);
                    StartCoroutine(startCountdown);
                }
                massMod = action.mass.addModifier(massNerf);
            }
            else
            {
                action.mass.removeModifier(massMod);
                massMod = null;
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

    public void Initialize(float duration, float massNerf, ContagionAbility origin, ContagionEffects effects)
    {
        this.duration = duration;
        this.origin = origin;
        this.effects = effects;
        this.massNerf = massNerf;
        action = GetComponent<InputToAction>();
    }

}
