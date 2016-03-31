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
        else if (Input.GetAxis(bindings.acceptAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.MOVEMENT);
            prevMovement = Input.GetAxis(bindings.acceptAbilityAxis);
        }
        if (Input.GetAxis(bindings.superAbilityAxis) != prevSuper)
        {
            Debug.Log(Input.GetAxis(bindings.superAbilityAxis));
            action.FireAbility(AbilityType.SUPER);
            prevSuper = Input.GetAxis(bindings.superAbilityAxis);
        }
        if (Input.GetAxis(bindings.genericAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.GENERIC);
            prevGeneric = Input.GetAxis(bindings.genericAbilityAxis);
        }
        else if (Input.GetAxis(bindings.backAbilityAxis) != 0)
        {
            action.FireAbility(AbilityType.GENERIC);
            prevGeneric = Input.GetAxis(bindings.backAbilityAxis);
        }
        if (prevMovement != 0 && Mathf.Max(Input.GetAxis(bindings.movementAbilityAxis), Input.GetAxis(bindings.acceptAbilityAxis)) == 0)
        {
            action.StopFireAbility(AbilityType.MOVEMENT);
            prevMovement = 0;
        }
        if (prevSuper == Input.GetAxis(bindings.superAbilityAxis))
        {
            action.StopFireAbility(AbilityType.SUPER);
        }
        if (prevGeneric != 0 && Mathf.Max(Input.GetAxis(bindings.genericAbilityAxis), Input.GetAxis(bindings.backAbilityAxis)) == 0)
        {
            action.StopFireAbility(AbilityType.GENERIC);
            prevGeneric = 0;
        }
    }
}
