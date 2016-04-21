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
        if (prevMovement == 0)
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
        }
            //prevMovement is != 0
        else if (Input.GetAxis(bindings.movementAbilityAxis) == 0 && Input.GetAxis(bindings.acceptAbilityAxis) == 0)
        {
            action.StopFireAbility(AbilityType.MOVEMENT);
            prevMovement = 0;
        }

        if (Input.GetAxis(bindings.superAbilityAxis) != prevSuper)
        {
            action.FireAbility(AbilityType.SUPER);
            prevSuper = Input.GetAxis(bindings.superAbilityAxis);
        }

        if (prevSuper == Input.GetAxis(bindings.superAbilityAxis))
        {
            action.StopFireAbility(AbilityType.SUPER);
        }

        if (prevGeneric == 0)
        {
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
        }
            //prev generic != 0
        else if (Input.GetAxis(bindings.genericAbilityAxis) == 0 && Input.GetAxis(bindings.backAbilityAxis) == 0)
        {
            action.StopFireAbility(AbilityType.GENERIC);
            prevGeneric = 0;
        }
    }

    public override bool pressedAccept()
    {
        //ability axis
        float currentAxisValue = Input.GetAxis(bindings.movementAbilityAxis);

        bool returnValue = false;
        if (currentAxisValue != 0)
        {
            returnValue = true;
        }

        //now XY axis
        currentAxisValue = Input.GetAxis(bindings.acceptAbilityAxis);

        if (currentAxisValue != 0)
        {
            returnValue = true;
        }

        return returnValue;
    }

    public override bool pressedBack()
    {
        //ability axis
        float currentAxisValue = Input.GetAxis(bindings.genericAbilityAxis);

        bool returnValue = false;
        if (currentAxisValue != 0)
        {
            returnValue = true;
        }

        //now XY axis
        currentAxisValue = Input.GetAxis(bindings.backAbilityAxis);

        if (currentAxisValue != 0)
        {
            returnValue = true;
        }

        return returnValue;
    }

    public override Vector2 deltaVisuals()
    {
        return new Vector2(Input.GetAxis(bindings.horizontalVisualsAxis), Input.GetAxis(bindings.verticalVisualsAxis));
    }
}
