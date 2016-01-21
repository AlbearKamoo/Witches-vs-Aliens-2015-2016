using UnityEngine;
using System.Collections;

public abstract class AbstractMovementAbility : NotSuperAbility
{

    public override AbilityType type { get { return AbilityType.MOVEMENT; } }

    protected override void onFire(Vector2 direction) 
    {
        //Debug.Log("MOVE!"); //override and remove this
    } 
}

public class MovementAbility : AbstractMovementAbility
{
    protected override void Reset()
    {
    }
}
