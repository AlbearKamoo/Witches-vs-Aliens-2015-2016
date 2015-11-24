using UnityEngine;
using System.Collections;

public class PushAbilityCircle : GenericAbility
{

    ParticleSystem vfx;
    CircleCollider2D coll;
    PointEffector2D effector;

    [SerializeField]
    protected float maxDuration;

    protected override void OnActivate()
    {
        base.OnActivate();
        vfx.Play();
        coll.enabled = true;
        effector.enabled = true;

    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        vfx.Stop();
        vfx.Clear();
        coll.enabled = false;
        effector.enabled = false;
    }

    // Use this for initialization
    void Awake()
    {
        vfx = GetComponent<ParticleSystem>();
        coll = GetComponent<CircleCollider2D>();
        effector = GetComponent<PointEffector2D>();
    }

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(UpdateCharge());
    }

    public override void StopFire()
    {
        //active = false;
        base.StopFire();
    }

    private IEnumerator UpdateCharge()
    {
        active = true;
        float duration = 0;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            duration += Time.fixedDeltaTime;
            if (duration > maxDuration)
            {
                active = false;
                yield break;
            }
        }
    }
}
