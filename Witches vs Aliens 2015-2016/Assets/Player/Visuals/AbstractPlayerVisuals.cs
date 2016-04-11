using UnityEngine;
using System.Collections;

public abstract class AbstractPlayerVisuals : MonoBehaviour {
    Rigidbody2D rigid;
	// Use this for initialization
    public abstract float alpha { get; set; }

	protected virtual void Start () {
        GetComponentInParent<VisualAnimate>().targets[0] = GetComponent<Renderer>();
        rigid = GetComponentInParent<Rigidbody2D>();
	}
	
	// Update is called once per frame
    protected virtual void Update()
    {
        Vector2 movementDirection = rigid.velocity;
        if (movementDirection != Vector2.zero)
            UpdateVisualRotation(movementDirection);
    }

    public abstract Sprite selectionSprite(Vector2 visualSpaceInput);

    public virtual Sprite selectionSprite()
    {
        return selectionSprite(Vector2.zero);
    }

    protected abstract void UpdateVisualRotation(Vector2 direction);
}
