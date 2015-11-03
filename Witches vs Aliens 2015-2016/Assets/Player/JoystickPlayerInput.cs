using UnityEngine;
using System.Collections;

public class JoystickPlayerInput : AbstractPlayerInput {
    float prevMovement = 0;
    float prevGeneric = 0;
    float prevSuper = 0;
    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => distance * aim;
        action.vectorToPercent = (Vector2 aim, float distance) => aim.magnitude;
    }

    protected override void updateAim()
    {
        action.aimingInputDirection = new Vector2(Input.GetAxis(bindings.horizontalAimingAxisName), Input.GetAxis(bindings.verticalAimingAxisName));
    }

    protected override void checkAbilities()
    {
        if (Input.GetAxis(bindings.movementAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.MOVEMENT);
            prevMovement = Input.GetAxis(bindings.movementAbilityAxis);
        }
        if (Input.GetAxis(bindings.superAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.SUPER);
            prevSuper = Input.GetAxis(bindings.superAbilityAxis);
        }
        if (Input.GetAxis(bindings.genericAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.GENERIC);
            prevGeneric = Input.GetAxis(bindings.genericAbilityAxis);
        }

        if (prevMovement != 0 && Input.GetAxis(bindings.movementAbilityAxis) == 0)
            action.StopFireAbility(AbilityType.MOVEMENT);
        if (prevSuper != 0 && Input.GetAxis(bindings.superAbilityAxis) == 0)
            action.StopFireAbility(AbilityType.SUPER);
        if (prevGeneric != 0 && Input.GetAxis(bindings.genericAbilityAxis) == 0)
            action.StopFireAbility(AbilityType.GENERIC);
    }
}
