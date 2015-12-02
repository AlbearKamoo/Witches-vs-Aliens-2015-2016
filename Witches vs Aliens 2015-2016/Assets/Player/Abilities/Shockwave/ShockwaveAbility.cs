using UnityEngine;
using System.Collections;

public class ShockwaveAbility : GenericAbility
{
    [SerializeField]
    protected float chargeUp;

    [SerializeField]
    protected float radius;

    ParticleSystem vfx;

    // Use this for initialization
    void Awake()
    {
        vfx = GetComponent<ParticleSystem>();
        vfx.startSize = radius;
    }

    protected override void onFire(Vector2 direction)
    {
        active = true;
        Callback.FireAndForget(() =>
        {
            vfx.Play();

            Vector2 thisPosition = this.transform.position;
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(thisPosition, radius))
            {
                ISpeedLimiter limiter = hit.GetComponentInParent<ISpeedLimiter>();
                if (limiter != null)
                {
                    Vector2 displacement = ((Vector2)(hit.transform.position)) - thisPosition;
                    if (displacement != Vector2.zero)
                    {
                        Rigidbody2D hitRigidbody = hit.GetComponentInParent<Rigidbody2D>();
                        displacement.Normalize();
                        hitRigidbody.velocity = limiter.maxSpeed * displacement;
                        if (hit.CompareTag(Tags.puck))
                            hit.GetComponent<LastBumped>().setLastBumped(this.transform.parent);
                    }
                }
            }
            active = false;
        }, chargeUp, this);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.puck))
        {
            other.GetComponent<LastBumped>().setLastBumped(this.transform.parent);
        }
    }
}