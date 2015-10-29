using UnityEngine;
using System.Collections;

public class JoystickPlayerInput : AbstractPlayerInput {

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
        if (Input.GetKeyDown(bindings.movementAbilityKey))
            action.FireAbility(AbilityType.MOVEMENT);
        if (Input.GetKeyDown(bindings.superAbilityKey))
            action.FireAbility(AbilityType.SUPER);
        if (Input.GetKeyDown(bindings.genericAbilityKey))
            action.FireAbility(AbilityType.GENERIC);

        if (Input.GetKeyUp(bindings.movementAbilityKey))
            action.StopFireAbility(AbilityType.MOVEMENT);
        if (Input.GetKeyUp(bindings.superAbilityKey))
            action.StopFireAbility(AbilityType.SUPER);
        if (Input.GetKeyUp(bindings.genericAbilityKey))
            action.StopFireAbility(AbilityType.GENERIC);

    }
}
