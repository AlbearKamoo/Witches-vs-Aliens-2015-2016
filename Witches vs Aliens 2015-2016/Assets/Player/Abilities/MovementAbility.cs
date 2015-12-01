using UnityEngine;
using System.Collections;

public class MovementAbility : NotSuperAbility
{

    public override AbilityType type { get { return AbilityType.MOVEMENT; } }

    protected override void onFire(Vector2 direction) 
    {
        //Debug.Log("MOVE!"); //override and remove this
    } 
}
