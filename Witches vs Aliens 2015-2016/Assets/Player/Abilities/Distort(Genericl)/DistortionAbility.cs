using UnityEngine;
using System.Collections;

public class DistortionAbility : TimedGenericAbility
{

    ParticleSystem vfx;
    CircleCollider2D coll;
    PointEffector2D effector;
    MeshRenderer render;
    Rigidbody2D rigid;

    float rigidMass;

    [SerializeField]
    protected bool affectsPlayers;

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

    public override void StopFire()
    {
        //active = false; lasts for full duration
        base.StopFire();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Tags.puck))
        {
            other.GetComponent<LastBumped>().setLastBumped(rigid.transform);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!affectsPlayers && other.CompareTag(Tags.player))
        {
            Physics2D.IgnoreCollision(other, coll);
        }
    }
}
