using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class BoostAbility : MovementAbility {

    InputToAction action;
    FloatStat speedMod;
    AudioSource sfx;
    ParticleSystem vfx;
    bool _active = false; //do NOT use; use the property because it has setters
    bool active { get { return _active; }
        set
        {


            if (value)
            {
                if (!_active)
                {
                    sfx.Play();
                    vfx.Play();
                }
            }
            else if (_active)
            {
                vfx.Stop();
            }
            _active = value;
        }
    }

    //may want to turn these into protected [SerializeField]s if the stats are going to be different for each char
    const float baseBoost = 2.5f;
    const float boostGain = 1.5f;
    const float boostCost = 0.75f;
    const float boostLoss = 3f;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        vfx = GetComponent<ParticleSystem>();
        GetComponent<ParticleSystemRenderer>().material.mainTexture = transform.parent.Find("Visuals").GetComponent<SpriteRenderer>().sprite.texture; //use the main texture of the object for the flash-trail fx
    }

    void Start()
    {
        action = GetComponentInParent<InputToAction>();
    }

    protected override void onFire()
    {
        base.onFire();
        StartCoroutine(Boost());
    }

    public override void StopFire()
    {
        active = false;
        base.StopFire();
    }

    private IEnumerator Boost()
    {
        if (speedMod == null)
            speedMod = action.maxSpeed.addSpeedModifier(baseBoost);
        else
            speedMod.value += (baseBoost - 1);
        active = true;
        sfx.Play();
        while (active)
        {
            yield return new WaitForFixedUpdate();
            speedMod.value += Time.fixedDeltaTime * boostGain;
            _charge -= Time.fixedDeltaTime * boostCost;
            if (_charge < 0)
            {
                _charge = 0;
                active = false;
            }
        }

        while (!active)
        {
            speedMod.value -= Time.fixedDeltaTime * boostLoss;
            if (speedMod.value < 1)
            {
                action.maxSpeed.removeSpeedModifier(speedMod);
                speedMod = null;
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
