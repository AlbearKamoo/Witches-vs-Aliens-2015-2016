using UnityEngine;
using System.Collections;

public class BlinkAbility : MovementAbility {

    CircleCollider2D bounds;
    InputToAction action;
    LayerMask stageMask;
    const float distance = 15f;
    const int numBolts = 3;

    public GameObject LightingFXPrefab;
	// Use this for initialization
	void Start () {
        bounds = GetComponentInParent<CircleCollider2D>();
        action = GetComponentInParent<InputToAction>();
        stageMask = LayerMask.GetMask(new string[]{Tags.Layers.stage});
	}

    protected override void onFire()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.parent.position, bounds.radius, action.normalizedInput, distance, stageMask);
        float jumpDistance = hit ? hit.distance: distance;
        Vector2 targetPos = (Vector2)(transform.parent.position) + jumpDistance * action.normalizedInput;

        for (int i = 0; i < numBolts; i++)
        {
            Debug.Log("BOLT");
            SimplePool.Spawn(LightingFXPrefab, transform.parent.position).GetComponent<Lightning>().DoFX(targetPos);
        }

        transform.parent.position = targetPos;
    }
}
