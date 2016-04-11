using UnityEngine;
using System.Collections;

public class ContinuousDirectionVisuals : AbstractPlayerVisuals {

    Transform thisTransform;
    SpriteRenderer rend;

    [SerializeField]
    protected Sprite _selectionSprite;

    public override Sprite selectionSprite(Vector2 visualSpaceInput) { return _selectionSprite; }

    public override float alpha
    {
        get
        {
            return rend.color.a;
        }
        set
        {
            rend.color = rend.color.setAlphaFloat(value);
        }
    }

    void Awake()
    {
        thisTransform = this.transform;
        rend = GetComponent<SpriteRenderer>();
    }

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        thisTransform.rotation = direction.ToRotation();
    }
}
