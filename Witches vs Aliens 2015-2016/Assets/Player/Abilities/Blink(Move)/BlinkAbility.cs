using UnityEngine;
using System.Collections;

public class BlinkAbility : MovementAbility {

    CircleCollider2D bounds;
    LayerMask stageMask;
    [SerializeField]
    protected float distance = 6f;
    [SerializeField]
    protected int numFXBolts = 10;

    public GameObject LightingFXPrefab;
	// Use this for initialization
	protected override void Start () {
        base.Start();
        bounds = transform.parent.GetComponentInChildren<CircleCollider2D>();
        stageMask = LayerMask.GetMask(new string[]{Tags.Layers.stage});
	}

    protected override void onFire(Vector2 direction)
    {
        active = true; //have to toggle it for parent classes to work correctly
        RaycastHit2D hit = Physics2D.CircleCast(transform.parent.position, bounds.radius, direction, distance * 1, stageMask);
        Vector2 displacement = hit ? hit.distance * direction.normalized : distance * direction.normalized;
        Vector2 targetPos = (Vector2)(transform.parent.position) + displacement;

        for (int i = 0; i < numFXBolts; i++)
        {
            SimplePool.Spawn(LightingFXPrefab, (Vector2)(transform.parent.position) + Random.insideUnitCircle * bounds.radius).GetComponent<Lightning>().DoFX(targetPos + Random.insideUnitCircle * bounds.radius);
        }

        transform.parent.position = targetPos;
        active = false;
    }
}
