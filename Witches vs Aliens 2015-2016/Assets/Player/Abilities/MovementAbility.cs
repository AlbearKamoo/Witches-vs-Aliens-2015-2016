using UnityEngine;
using System.Collections;

public class MovementAbility : AbstractAbility {

    public override AbilityType type { get { return AbilityType.MOVEMENT; } }

    protected override void onFire() 
    {
        Debug.Log("MOVE!"); 
    } 
}
