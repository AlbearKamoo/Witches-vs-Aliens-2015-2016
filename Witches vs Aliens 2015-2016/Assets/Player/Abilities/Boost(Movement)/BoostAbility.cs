using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class BoostAbility : MovementAbility {

    InputToAction action;
    FloatStat speedMod;
    FloatStat accelMod;
    AudioSource sfx;
    ParticleSystem vfx;
    Rigidbody2D rigid;
    bool _active = false; //do NOT use; use the property because it has setters
    bool active { get { return _active; }
        set
        {
            if (value)
            {
                if (!_active)
                {
                    sfx.Play();
                    StartCoroutine(playFX());
                }
            }
                /*
            else if (_active)
            {
                
            }
                 */
            _active = value;
        }
    }

    //may want to turn these into protected [SerializeField]s if the stats are going to be different for each char
    const float baseBoostSpeedMultiplier = 2.5f;
    const float boostCostPerSec = 1.75f;
    const float boostDecayRate = 3f;
    const float baseAccelNerf = 0.066f;
    const float accelNerfDecayRate = 1f;
    const float minFXTime = 0.5f;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        vfx = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        action = GetComponentInParent<InputToAction>();
        vfx.startSize = 2*transform.parent.GetComponentInChildren<CircleCollider2D>().radius;
        rigid = GetComponentInParent<Rigidbody2D>();
    }

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(Boost(direction));
    }

    public override void StopFire()
    {
        active = false;
        base.StopFire();
    }

    IEnumerator Boost(Vector2 direction)
    {
        if (speedMod == null)
            speedMod = action.maxSpeed.addSpeedModifier(baseBoostSpeedMultiplier);
        else
            speedMod.value = baseBoostSpeedMultiplier;

        if (accelMod == null)
            accelMod = action.accel.addSpeedModifier(baseAccelNerf);
        else
            accelMod.value = baseAccelNerf;

        rigid.velocity = action.maxSpeed * direction.normalized;

        active = true;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            _charge -= Time.fixedDeltaTime * boostCostPerSec;
            if (_charge < 0)
            {
                _charge = 0;
                active = false;
            }
        }

        StartCoroutine(DecaySpeed());
        StartCoroutine(DecayAccel());
    }
    IEnumerator DecaySpeed()
    {
        while (!active)
        {
            speedMod.value -= Time.fixedDeltaTime * boostDecayRate;
            if (speedMod.value < 1)
            {
                action.maxSpeed.removeSpeedModifier(speedMod);
                speedMod = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator DecayAccel()
    {
        while (!active)
        {
            accelMod.value += Time.fixedDeltaTime * accelNerfDecayRate;
            if (accelMod.value > 1)
            {
                action.accel.removeSpeedModifier(accelMod);
                accelMod = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator playFX()
    {
        vfx.Play();
        float time = 0;
        while (active || time < minFXTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        vfx.Stop();
    }


}
