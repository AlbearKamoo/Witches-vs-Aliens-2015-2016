using UnityEngine;
using System.Collections;

public abstract class AbstractPlayerVisuals : MonoBehaviour {
    Rigidbody2D rigid;
	// Use this for initialization
	void Start () {
        GetComponentInParent<VisualAnimate>().targets[0] = GetComponent<Renderer>();
        rigid = GetComponentInParent<Rigidbody2D>();
	}
	
	// Update is called once per frame
    void Update()
    {
        Vector2 movementDirection = rigid.velocity;
        if (movementDirection != Vector2.zero)
            UpdateVisualRotation(movementDirection);
    }

    protected abstract void UpdateVisualRotation(Vector2 direction);
}
