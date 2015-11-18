using UnityEngine;
using System.Collections;

public class RandomizedSupersGoal : SuperGoal {

    [SerializeField]
    protected GameObject[] leftSupers;

    [SerializeField]
    protected GameObject[] rightSupers;

    protected override void activateSuper(LastBumped bumped)
    {
        GameObject newAbility = null;
        switch (bumped.side)
        {
            case Side.LEFT:
                newAbility = SimplePool.Spawn(leftSupers[Random.Range(0, leftSupers.Length)]);
                break;
            case Side.RIGHT:
                newAbility = SimplePool.Spawn(rightSupers[Random.Range(0, rightSupers.Length)]);
                break;
        }
        newAbility.transform.SetParent(bumped.player, false);
        SuperAbility newSuper = GetComponent<SuperAbility>();
        newSuper.ready = true;
        bumped.player.GetComponent<InputToAction>().SuperAbility = newSuper;
    }
}
