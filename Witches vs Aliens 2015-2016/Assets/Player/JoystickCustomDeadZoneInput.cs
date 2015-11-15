using UnityEngine;
using System.Collections;

public class JoystickCustomDeadZoneInput : JoystickPlayerInput {
    const float deadZone = 0.25f;
    const float deadZoneSqr = deadZone * deadZone; //approx 0.05f;
    protected override void updateAim()
    {
        Vector2 aimingInput = new Vector2(Input.GetAxis(bindings.horizontalAimingAxisName), Input.GetAxis(bindings.verticalAimingAxisName));
        if (aimingInput.sqrMagnitude > deadZoneSqr)
        {
            //continuous transition from dead zone to not-dead zone
            action.aimingInputDirection = ((aimingInput.magnitude - deadZone)/(1-deadZone)) * aimingInput.normalized;
            //action.aimingInputDirection = aimingInput;
        }
        else
            action.aimingInputDirection = Vector2.zero;
    }

    protected override void updateMovement()
    {
        Vector2 movementInput = new Vector2(Input.GetAxis(bindings.horizontalMovementAxisName), Input.GetAxis(bindings.verticalMovementAxisName));
        if (movementInput.sqrMagnitude > deadZoneSqr)
            action.normalizedMovementInput = Vector2.ClampMagnitude(movementInput, 1);
        else
            action.normalizedMovementInput = Vector2.zero;
    }
}
