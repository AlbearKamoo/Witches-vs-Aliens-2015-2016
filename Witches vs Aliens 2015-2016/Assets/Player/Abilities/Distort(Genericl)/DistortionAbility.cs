using UnityEngine;
using System.Collections;

public class DistortionAbility : GenericAbility {

    ParticleSystem vfx;
    CircleCollider2D coll;
    PointEffector2D effector;
    MeshRenderer render;
    Rigidbody2D rigid;

    float rigidMass;

    [SerializeField]
    protected float maxDuration;

    protected override void OnActivate()
    {
        base.OnActivate();
        vfx.Play();
        coll.enabled = true;
        effector.enabled = true;
        render.enabled = true;
        rigid.mass = 99999f;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        vfx.Stop();
        vfx.Clear();
        coll.enabled = false;
        effector.enabled = false;
        render.enabled = false;
        rigid.mass = rigidMass;
    }

    // Use this for initialization
    void Awake()
    {
        vfx = GetComponent<ParticleSystem>();
        coll = GetComponent<CircleCollider2D>();
        effector = GetComponent<PointEffector2D>();
        render = GetComponent<MeshRenderer>();
	}

    protected override void Start()
    {
        rigid = GetComponentInParent<Rigidbody2D>();
        rigidMass = rigid.mass;
        base.Start();
    }

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(UpdateCharge());
    }

    public override void StopFire()
    {
        active = false;
        base.StopFire();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.puck))
        {
            other.GetComponent<LastBumped>().setLastBumped(rigid.transform);
        }
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
