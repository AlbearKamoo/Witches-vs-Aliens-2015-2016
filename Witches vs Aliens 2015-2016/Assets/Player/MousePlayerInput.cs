using UnityEngine;
using System.Collections;

public class MousePlayerInput : AbstractPlayerInput {

    Transform thisTransform;

    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => Vector2.ClampMagnitude(aim, distance);
        action.vectorToPercent = (Vector2 aim, float distance) => Mathf.Clamp01(aim.magnitude / distance);
    }

    void Awake()
    {
        thisTransform = this.transform;
    }

    protected override void updateAim()
    {
        action.aimingInputDirection = Format.mousePosInWorld() - thisTransform.position;
    }

    protected override void checkAbilities()
    {
        if (Input.GetMouseButtonDown(0))
            action.FireAbility(AbilityType.MOVEMENT);
        if (Input.GetMouseButtonDown(1))
            action.FireAbility(AbilityType.GENERIC);
        if (Input.GetMouseButtonDown(2))
            action.FireAbility(AbilityType.SUPER); //not all mouses have middle mouse buttons; might want to change

        if (Input.GetMouseButtonUp(0))
            action.StopFireAbility(AbilityType.MOVEMENT);
        if (Input.GetMouseButtonUp(1))
            action.StopFireAbility(AbilityType.GENERIC);
        if (Input.GetMouseButtonUp(2))
            action.StopFireAbility(AbilityType.SUPER);

    }

    public override bool pressedAccept()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
    }

    public override bool pressedBack()
    {
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);
    }

    public override Vector2 deltaVisuals()
    {
        return 10 * Input.GetAxis("Mouse ScrollWheel") * Vector2.right;
    }
}

    

