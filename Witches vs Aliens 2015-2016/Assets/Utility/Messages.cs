using UnityEngine;
using System.Collections;

public class AbilityStateChangedMessage
{
    public readonly bool ready;
    public readonly AbilityType type;
    public AbilityStateChangedMessage(bool ready, AbilityType type)
    {
        this.ready = ready;
        this.type = type;
    }
}