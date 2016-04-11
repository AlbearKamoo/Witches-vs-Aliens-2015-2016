using UnityEngine;
using System.Collections;

public class CrappyAIInput : AbstractPlayerInput
{
    Transform puckTransform;
    Transform thisTransform;
    void Awake()
    {
        puckTransform = GameObject.FindGameObjectWithTag(Tags.puck).transform;
        thisTransform = this.transform;
    }

    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => Vector2.ClampMagnitude(aim, distance);
        action.vectorToPercent = (Vector2 aim, float distance) => Mathf.Clamp01(aim.magnitude / distance);
    }

    protected override void updateMovement()
    {
        action.normalizedMovementInput = (puckTransform.position - thisTransform.position).normalized;
    }

    protected override void updateAim()
    {
        action.aimingInputDirection = (puckTransform.position - thisTransform.position);
    }

    protected override void checkAbilities()
    {

    }

    public override bool pressedAccept()
    {
        return false;
    }

    public override bool pressedBack()
    {
        return false;
    }

    public override Vector2 deltaVisuals()
    {
        return Vector2.zero;
    }
}
