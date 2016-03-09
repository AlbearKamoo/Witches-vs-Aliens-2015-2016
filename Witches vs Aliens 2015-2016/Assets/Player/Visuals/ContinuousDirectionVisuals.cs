using UnityEngine;
using System.Collections;

public class ContinuousDirectionVisuals : AbstractPlayerVisuals {

    Transform thisTransform;

    [SerializeField]
    protected Sprite _selectionSprite;

    public override Sprite selectionSprite(Vector2 visualSpaceInput) { return _selectionSprite; }

    void Awake()
    {
        thisTransform = this.transform;
    }

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        thisTransform.rotation = direction.ToRotation();
    }
}
