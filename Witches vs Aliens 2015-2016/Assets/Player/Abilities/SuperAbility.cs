using UnityEngine;
using System.Collections;

public class SuperAbility : AbstractAbility
{
    public override AbilityType type { get { return AbilityType.SUPER; } }
    protected override void onFire(Vector2 direction)
    {
        Debug.Log("SUPER!");
    }
}
