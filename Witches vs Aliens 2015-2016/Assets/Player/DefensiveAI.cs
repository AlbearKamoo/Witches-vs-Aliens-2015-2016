using UnityEngine;
using System.Collections;
using System.Linq;
public class DefensiveAI : AbstractAI, GoalAI
{
    Transform _myGoal;
    public Transform myGoal { set { _myGoal = value; } }
    Transform _opponentGoal;
    public Transform opponentGoal { set { _opponentGoal = value; } }

    const float chargeDistance = 0.75f;

    protected override void updateMovement()
    {
        Vector2 puckPos = puckTransform.position;

        Vector2 nearestOnLinePoint = ClosestPointOnLine(_myGoal.position, puckPos, thisTransform.position);
        if (OnLine(_myGoal.position, puckPos, nearestOnLinePoint))
        {
            if (Vector2.Distance(nearestOnLinePoint, thisTransform.position) < chargeDistance)
                SetTargetVector(puckPos);
            else
                SetTargetVector(nearestOnLinePoint);
        }
        else
        {
            SetTargetVector(_myGoal.position);
        }
    }
    protected override void updateAim()
    {
        action.aimingInputDirection = (puckTransform.position - thisTransform.position);
    }
}
