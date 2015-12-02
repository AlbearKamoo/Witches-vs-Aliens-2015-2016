using UnityEngine;
using System.Collections;

public class NullAbilityUI : AbstractAbilityUI
{
    public override void Construct(AbilityUIConstructorInfo info)
    { }
    public override void Notify(AbilityStateChangedMessage m) //update our display state
    { }

    public override void Notify(ResetMessage m) //when this happens, disable all distance-emission particle effects for one frame because the player is about to teleport
    { }

}
