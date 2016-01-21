using UnityEngine;
using System.Collections;

public class DistortionAbility : TimedGenericAbility
{

    ParticleSystem vfx;
    CircleCollider2D coll;
    PointEffector2D effector;
    MeshRenderer render;
    InputToAction action;
    FloatStat massMod;

    [SerializeField]
    protected bool affectsPlayers;

    protected override void OnActivate()
    {
        base.OnActivate();
        vfx.Play();
        coll.enabled = true;
        effector.enabled = true;
        render.enabled = true;
        massMod = action.mass.addModifier(99999f);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        vfx.Stop();
        vfx.Clear();
        coll.enabled = false;
        effector.enabled = false;
        render.enabled = false;
        action.mass.removeModifier(massMod);
        massMod = null;
    }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        vfx = GetComponent<ParticleSystem>();
        coll = GetComponent<CircleCollider2D>();
        effector = GetComponent<PointEffector2D>();
        render = GetComponent<MeshRenderer>();
	}

    protected override void Start()
    {
        action = GetComponentInParent<InputToAction>();
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
            other.GetComponent<LastBumped>().setLastBumped(transform.root);
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
