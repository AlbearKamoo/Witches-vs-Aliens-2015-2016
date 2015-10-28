using UnityEngine;
using System.Collections;

public class DistortionAbility : GenericAbility {

    ParticleSystem vfx;
    CircleCollider2D coll;
    PointEffector2D effector;
    MeshRenderer render;

    const float activeCost = 1f;

    bool _active = false; //do NOT use; use the property because it has setters
    bool active
    {
        get { return _active; }
        set
        {
            if (value)
            {
                if (!_active)
                {
                    vfx.Play();
                    coll.enabled = true;
                    effector.enabled = true;
                    render.enabled = true;
                }
            }
            else if (_active)
            {
                vfx.Stop();
                vfx.Clear();
                coll.enabled = false;
                effector.enabled = false;
                render.enabled = false;
            }
            _active = value;
        }
    }

    // Use this for initialization
	void Awake () {
        vfx = GetComponent<ParticleSystem>();
        coll = GetComponent<CircleCollider2D>();
        effector = GetComponent<PointEffector2D>();
        render = GetComponent<MeshRenderer>();
	}

    protected override void onFire(Vector2 direction)
    {
        _charge += 1; //charge was decremented by 1 when fired; this ensures that there is no net change, but you need at least one charge to fire
        StartCoroutine(UpdateCharge());
    }

    public override void StopFire()
    {
        active = false;
        base.StopFire();
    }

    private IEnumerator UpdateCharge()
    {
        active = true;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            _charge -= Time.fixedDeltaTime * activeCost;
            if (_charge < 0)
            {
                _charge = 0;
                active = false;
                yield break;
            }
        }
    }
}
