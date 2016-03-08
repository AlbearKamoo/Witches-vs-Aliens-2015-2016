using UnityEngine;
using System.Collections;

public abstract class AbstractGenericAbility : NotSuperAbility
{
    public override AbilityType type { get { return AbilityType.GENERIC; } }
    protected override void onFire(Vector2 direction)
    {
        Debug.Log("GENERIC!"); //override and remove this
    }
}

public class GenericAbility : AbstractGenericAbility
{
    protected override void Reset(float timeTillActive)
    {
    }
}