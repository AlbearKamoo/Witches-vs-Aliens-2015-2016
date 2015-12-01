using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioController))]
[RequireComponent(typeof(ParticleSystem))]
public class BoostAbility : MovementAbility, IObserver<ResetMessage> {

    InputToAction action;
    FloatStat speedMod;
    FloatStat accelMod;
    AudioController sfx;
    ParticleSystem vfx;
    Rigidbody2D rigid;

    float rigidMass;

    const float massCoefficient = 3f;

    protected override void OnActivate()
    {
        base.OnActivate();
        sfx.Play();
        StartCoroutine(playFX());
    }

    //may want to turn these into protected [SerializeField]s if the stats are going to be different for each char
    [SerializeField]
    protected float speedMultiplier = 2.5f;
    [SerializeField]
    protected float maxDuration = 0.5f;
    [SerializeField]
    protected float boostDecayTime = 1f;
    [SerializeField]
    protected float baseAccelDebuff = 0.066f;
    [SerializeField]
    protected float accelNerfDecayTime = 1f;
    [SerializeField]
    protected float FXDurationExtend = 0.5f;

    void Awake()
    {
        sfx = GetComponent<AudioController>();
        vfx = GetComponent<ParticleSystem>();
    }

    protected override void Start()
    {
        base.Start();
        action = GetComponentInParent<InputToAction>();
        vfx.startSize = 2*transform.parent.GetComponentInChildren<CircleCollider2D>().radius;
        rigid = GetComponentInParent<Rigidbody2D>();
        GetComponentInParent<IObservable<ResetMessage>>().Observable().Subscribe(this);
        rigidMass = rigid.mass;
    }

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(Boost(direction));
    }

    public override void StopFire()
    {
        //active = false;
        base.StopFire();
    }

    IEnumerator Boost(Vector2 direction)
    {
        if (speedMod == null)
            speedMod = action.maxSpeed.addModifier(speedMultiplier);
        else
            speedMod.value = speedMultiplier;

        if (accelMod == null)
            accelMod = action.accel.addModifier(baseAccelDebuff);
        else
            accelMod.value = baseAccelDebuff;

        rigid.velocity = action.maxSpeed * direction.normalized;
        rigid.mass = rigidMass * massCoefficient;

        active = true;
        float duration = 0;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            duration += Time.fixedDeltaTime;
            if (duration > maxDuration)
            {
                active = false;
            }
        }

        StartCoroutine(DecaySpeed());
        StartCoroutine(DecayAccel());
    }
    IEnumerator DecaySpeed()
    {
        float time = 0;
        while (!active)
        {
            time += Time.fixedDeltaTime;
            float lerpValue = time / boostDecayTime;
            speedMod.value = Mathf.Lerp(speedMultiplier, 1, lerpValue);
            rigid.mass = Mathf.Lerp(rigidMass * massCoefficient, rigidMass, lerpValue);
            if (time > boostDecayTime)
            {
                action.maxSpeed.removeModifier(speedMod);
                speedMod = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator DecayAccel()
    {
        float time = 0;
        while (!active)
        {
            time += Time.fixedDeltaTime;
            accelMod.value = Mathf.Lerp(baseAccelDebuff, 1, time / accelNerfDecayTime);
            if (time > boostDecayTime)
            {
                action.accel.removeModifier(accelMod);
                accelMod = null;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator playFX()
    {
        vfx.Play();
        while (active)
            yield return null;
        Callback.FireAndForget(() => vfx.Stop(), FXDurationExtend, this);
    }

    public void Notify(ResetMessage m)
    {
        vfx.Stop();
    }

}
