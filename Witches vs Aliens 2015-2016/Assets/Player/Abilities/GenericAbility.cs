using UnityEngine;
using System.Collections;

public abstract class GenericAbility : NotSuperAbility
{
    public override AbilityType type { get { return AbilityType.GENERIC; } }
    protected override void onFire(Vector2 direction)
    {
        Debug.Log("GENERIC!"); //override and remove this
    }
}
