using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class MobileAvoidBoid : MonoBehaviour, IMobileAvoidBoid {

    float _radius;
    public float radius { get { return _radius; } }

    Rigidbody2D rigid;
    public Vector2 velocity { get { return rigid.velocity; } }
    public Vector2 position { get { return transform.position; } }

	// Use this for initialization
	void Start () {
        _radius = GetComponent<CircleCollider2D>().radius;
        rigid = GetComponentInParent<Rigidbody2D>();
	}
}
