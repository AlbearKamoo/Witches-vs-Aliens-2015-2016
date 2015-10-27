using UnityEngine;
using System.Collections;

public class MousePlayerInput : AbstractPlayerInput {

    Transform thisTransform;

    void Awake()
    {
        thisTransform = this.transform;
    }

    protected override void updateAim()
    {
        action.aimingInput = Format.mousePosInWorld() - thisTransform.position;
    }

    protected override void checkAbilities()
    {
        if (Input.GetMouseButtonDown(0))
            action.FireAbility(AbilityType.MOVEMENT);
        if (Input.GetMouseButtonDown(1))
            action.FireAbility(AbilityType.SUPER);

        if (Input.GetMouseButtonUp(0))
            action.StopFireAbility(AbilityType.MOVEMENT);
        if (Input.GetMouseButtonUp(1))
            action.StopFireAbility(AbilityType.SUPER);
    }
}

    

