using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class RayDecal : MonoBehaviour {

    [SerializeField]
    protected float maxDistanceFromCenter;

    [SerializeField]
    protected float maxScale;

	// Use this for initialization
	void Awake () {
        Vector2 direction = Random.insideUnitCircle;
        this.transform.rotation = direction.ToRotation();
        this.transform.localPosition = maxDistanceFromCenter * direction;
        this.transform.localScale = new Vector2(Mathf.Lerp(1, maxScale, Random.value), 1);
	}
}
