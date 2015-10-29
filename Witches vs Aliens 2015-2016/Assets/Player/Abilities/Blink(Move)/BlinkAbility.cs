using UnityEngine;
using System.Collections;

public class BlinkAbility : MovementAbility {

    CircleCollider2D bounds;
    InputToAction action;
    LayerMask stageMask;
    [SerializeField]
    protected float distance = 6f;
    [SerializeField]
    protected int numFXBolts = 10;

    public GameObject LightingFXPrefab;
	// Use this for initialization
	void Start () {
        bounds = transform.parent.GetComponentInChildren<CircleCollider2D>();
        action = GetComponentInParent<InputToAction>();
        stageMask = LayerMask.GetMask(new string[]{Tags.Layers.stage});
	}

    protected override void onFire(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.parent.position, bounds.radius, direction, distance, stageMask);
        float jumpDistance = hit ? hit.distance: distance;
        Vector2 targetPos = (Vector2)(transform.parent.position) + jumpDistance * direction.normalized;

        for (int i = 0; i < numFXBolts; i++)
        {
            SimplePool.Spawn(LightingFXPrefab, transform.parent.position).GetComponent<Lightning>().DoFX(targetPos + Random.insideUnitCircle * bounds.radius);
        }

        transform.parent.position = targetPos;
    }
}
