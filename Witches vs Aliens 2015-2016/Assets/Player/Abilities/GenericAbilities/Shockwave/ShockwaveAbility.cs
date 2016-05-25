using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ShockwaveAbility : AbstractGenericAbility
{
    [SerializeField]
    protected float chargeUp;

    [SerializeField]
    protected float radius;

    [SerializeField]
    protected GameObject wavevfx;

    [SerializeField]
    [AutoLink(childPath = "Charging")]
    protected ParticleSystem chargingvfx;

    [SerializeField]
    protected AudioClip chargingSFX;

    [SerializeField]
    protected float backgroundMagnitude;

    [SerializeField]
    [AutoLink(childPath = "Charging/Background")]
    protected Transform background;

    [SerializeField]
    protected bool affectsPlayers;

    AudioSource sfx;

    float particleStartLifetime;
    Vector2 initialLocalScale;
    float progress;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        sfx = GetComponent<AudioSource>();
        foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
        {
            particles.startSize = 2 * radius;
        }

        particleStartLifetime = chargingvfx.startLifetime;
        initialLocalScale = background.localScale;
    }

    protected override void onFire(Vector2 direction)
    {
        active = true;
        chargingvfx.playbackSpeed = 1f;
        chargingvfx.Play();

        sfx.PlayOneShot(chargingSFX);

        background.gameObject.SetActive(true);
        Vector2 initialLocalScale = background.localScale;

        Callback.DoLerp((float l) =>
        {
            progress = l;
            background.localScale = initialLocalScale - backgroundMagnitude * Mathf.Sin(10 * Mathf.PI * l) * Vector2.one;
            chargingvfx.startSize = 1 + (2 * radius * l);
            chargingvfx.startLifetime = particleStartLifetime * l;
        }, chargeUp, this).FollowedBy(() =>
        {
            background.localScale = initialLocalScale;

            StopChargingFX();

            if (active)
            {
                Instantiate(wavevfx, this.transform.position, Quaternion.identity);

                Vector2 thisPosition = this.transform.position;
                foreach (Collider2D hit in Physics2D.OverlapCircleAll(thisPosition, radius))
                {
                    ISpeedLimiter limiter = hit.GetComponentInParent<ISpeedLimiter>();
                    if (limiter != null)
                    {
                        if (!affectsPlayers && hit.CompareTag(Tags.player))
                            continue;

                        Vector2 displacement = ((Vector2)(hit.transform.position)) - thisPosition;
                        if (displacement != Vector2.zero)
                        {
                            Rigidbody2D hitRigidbody = hit.GetComponentInParent<Rigidbody2D>();
                            displacement.Normalize();
                            hitRigidbody.velocity = limiter.maxSpeed * displacement;
                            if (hit.CompareTag(Tags.puck))
                                hit.GetComponent<LastBumped>().setLastBumped(this.transform.root);
                        }
                    }
                }
                active = false;
            }
        }, this);
    }

    protected override bool onFireActive(Vector2 direction)
    {
        background.localScale = initialLocalScale;

        StopChargingFX();

        GameObject instantiatedWave = Instantiate(wavevfx, this.transform.position, Quaternion.identity) as GameObject;
        instantiatedWave.GetComponent<ShockwaveWave>().scale = progress;
        Vector2 thisPosition = this.transform.position;
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(thisPosition, progress * radius))
        {
            ISpeedLimiter limiter = hit.GetComponentInParent<ISpeedLimiter>();
            if (limiter != null)
            {
                if (!affectsPlayers && hit.CompareTag(Tags.player))
                    continue;

                Vector2 displacement = ((Vector2)(hit.transform.position)) - thisPosition;
                if (displacement != Vector2.zero)
                {
                    Rigidbody2D hitRigidbody = hit.GetComponentInParent<Rigidbody2D>();
                    displacement.Normalize();
                    hitRigidbody.velocity = limiter.maxSpeed * displacement;
                    if (hit.CompareTag(Tags.puck))
                        hit.GetComponent<LastBumped>().setLastBumped(this.transform.root);
                }
            }
        }
        active = false;

        return true;
    }


    void StopChargingFX()
    {
        background.gameObject.SetActive(false);

        chargingvfx.Stop();
        chargingvfx.playbackSpeed = 10f; //end it gracefully

        sfx.Stop();
    }

    protected override void Reset(float timeTillActive)
    {
        active = false;
        StopChargingFX();
    }
    /*
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.puck))
        {
            other.GetComponent<LastBumped>().setLastBumped(this.transform.parent);
        }
    }
     * */
}