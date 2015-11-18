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
	void Awake()
	{
		vfx = GetComponent<ParticleSystem>();
		coll = GetComponent<PolygonCollider2D>();
		effector = GetComponent<PointEffector2D>();
	}
	
	protected override void onFire(Vector2 direction)
	{
		StartCoroutine(UpdateCharge());
		
		float angle = Mathf.Atan2 ( direction.y, direction.x );
		print (direction.ToString ());
		coll.transform.rotation = Quaternion.Euler (0f, 0f, 0f);
		coll.transform.Rotate(new Vector3 (0f,0f,angle * Mathf.Rad2Deg ));
		print (coll.transform.rotation.ToString());
		/*if (direction.x > 0) {
			float angle = Mathf.Atan(direction.normalized.x/direction.normalized.y);
		}*/
	}
	
	public override void StopFire()
	{
		active = false;
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
