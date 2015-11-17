using UnityEngine;
using System.Collections;

public class ContinuousDirectionVisuals : AbstractPlayerVisuals {

    Transform thisTransform;

    void Awake()
    {
        thisTransform = this.transform;
    }

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        thisTransform.rotation = direction.ToRotation();
    }
}
