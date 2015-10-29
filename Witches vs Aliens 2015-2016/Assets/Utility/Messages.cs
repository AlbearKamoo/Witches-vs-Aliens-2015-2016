using UnityEngine;
using System.Collections;

public class AbilityStateChangedMessage
{
    public readonly bool ready;
    public AbilityStateChangedMessage(bool ready)
    {
        this.ready = ready;
    }
}