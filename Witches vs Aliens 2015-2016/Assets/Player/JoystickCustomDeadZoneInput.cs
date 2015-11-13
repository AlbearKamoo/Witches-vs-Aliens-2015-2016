using UnityEngine;
using System.Collections;

public class JoystickCustomDeadZoneInput : JoystickPlayerInput {
    [SerializeField]
    protected float deadZone;
    float deadZoneSqr;
    protected void Awake()
    {
        deadZoneSqr = Mathf.Pow(deadZone, 2);
    }
    protected override void updateAim()
    {
        Vector2 aimingInput = new Vector2(Input.GetAxis(bindings.horizontalAimingAxisName), Input.GetAxis(bindings.verticalAimingAxisName));
        if (aimingInput.sqrMagnitude > deadZoneSqr)
            action.aimingInputDirection = aimingInput;
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
