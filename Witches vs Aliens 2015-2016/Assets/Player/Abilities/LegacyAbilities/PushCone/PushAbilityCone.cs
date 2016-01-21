using UnityEngine;
using System.Collections;

public class PushAbilityCone : GenericAbility {
	
	ParticleSystem vfx;
	PolygonCollider2D coll;
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
    protected override void Awake()
    {
        base.Awake();
		vfx = GetComponent<ParticleSystem>();
		coll = GetComponent<PolygonCollider2D>();
		effector = GetComponent<PointEffector2D>();
	}
	
	protected override void onFire(Vector2 direction)
	{
		StartCoroutine(UpdateCharge());
		
		float angle = Mathf.Atan2 ( direction.normalized.y, direction.normalized.x );
        
		coll.transform.rotation = Quaternion.Euler (0f, 0f, 0f);
        coll.transform.Rotate(new Vector3 (0f,0f,angle * Mathf.Rad2Deg ));
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

    protected override void Reset()
    {
        active = false;
    }
}
