using UnityEngine;
using System.Collections;

public class Interrupt : MonoBehaviour {

    Stats myStats;

    public bool active { get; set; }

    void Awake()
    {
        myStats = GetComponent<Stats>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (active)
        {
            foreach (IInterruptable i in col.transform.root.GetComponentsInChildren<IInterruptable>())
                i.Interrupt(myStats.side);
        }
    }
}

public interface IInterruptable
{
    void Interrupt(Side side);
}