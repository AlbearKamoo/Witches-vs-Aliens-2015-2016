using UnityEngine;
using System.Collections;

public class RandomizedSupersGoal : SuperGoal {

    [SerializeField]
    protected GameObject[] leftSupers;

    [SerializeField]
    protected GameObject[] rightSupers;

    ProgrammaticSpawning spawning;

    protected override void Awake()
    {
        base.Awake();
        foreach (GameObject controller in GameObject.FindGameObjectsWithTag(Tags.gameController))
        {
            spawning = controller.GetComponent<ProgrammaticSpawning>();
            if (spawning != null)
                break; //we've found it
        }
    }

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
        SuperAbility ability = newAbility.GetComponent<SuperAbility>();
        Callback.FireForNextFrame(() => ability.ready = true, this); //allow it to set up via start
        bumped.player.GetComponent<InputToAction>().SuperAbility = ability;

        if (ability is IAlliesAbility)
        {
            switch (bumped.side)
            {
                case Side.LEFT:
                    (ability as IAlliesAbility).allies = spawning.LeftPlayers;
                    break;
                case Side.RIGHT:
                    (ability as IAlliesAbility).allies = spawning.RightPlayers;
                    break;
            }
        }
        if (ability is IOpponentsAbility)
        {
            switch (bumped.side)
            {
                case Side.LEFT:
                    (ability as IOpponentsAbility).opponents = spawning.RightPlayers;
                    break;
                case Side.RIGHT:
                    (ability as IOpponentsAbility).opponents = spawning.LeftPlayers;
                    break;
            }
        }
        if (ability is IGoalAbility)
        {
            switch (bumped.side)
            {
                case Side.LEFT:
                    (ability as IGoalAbility).myGoal = spawning.LeftGoal;
                    (ability as IGoalAbility).opponentGoal = spawning.RightGoal;
                    break;
                case Side.RIGHT:
                    (ability as IGoalAbility).myGoal = spawning.RightGoal;
                    (ability as IGoalAbility).opponentGoal = spawning.LeftGoal;
                    break;
            }
        }
        if (ability is IPuckAbility)
        {
            (ability as IPuckAbility).puck = spawning.Puck;
        }
    }
}
