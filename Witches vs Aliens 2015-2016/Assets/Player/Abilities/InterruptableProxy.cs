using UnityEngine;
using System.Collections;

public class InterruptableProxy : MonoBehaviour, IInterruptable {

    public IInterruptableAbility source;

    public void Interrupt(Side side)
    {
        source.Interrupt(side);
    }
}

public interface IInterruptableAbility
{
    void Interrupt(Side side);
}