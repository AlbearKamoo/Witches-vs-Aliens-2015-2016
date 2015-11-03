using UnityEngine;
using System.Collections;
using System.Linq;
public class DefensiveAI : AbstractAI, GoalAI
{
    Transform _myGoal;
    public Transform myGoal { set { _myGoal = value; } }
    Transform _opponentGoal;
    public Transform opponentGoal { set { _opponentGoal = value; } }
    Rigidbody2D puckPhysics;
    LayerMask walls;
    Side side;

    protected override void Awake()
    {
        base.Awake();
        walls = LayerMask.GetMask(new string[]{Tags.Layers.stage});
    }

    protected override void Start()
    {
        base.Start();
        puckPhysics = puckTransform.GetComponent<Rigidbody2D>();
        side = GetComponent<Stats>().side;
    }

    const float chargeDistance = 0.75f;

    protected override void updateMovement()
    {
        Vector2 puckPos = puckTransform.position;
        if (side.onSide(Physics2D.Raycast(puckPos, puckPhysics.velocity, distance: float.MaxValue, layerMask: walls).point))
        {
            PlayDefensive(puckPos);
        }
        else
        {
            PlayOffensive(puckPos);
        }
    }

    void PlayDefensive(Vector2 puckPos)
    {
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
            action.FireAbility(AbilityType.MOVEMENT);
        }
        action.aimingInputDirection = (_myGoal.position - thisTransform.position);
    }

    void PlayOffensive(Vector2 puckPos)
    {
        Vector2 target = puckPos + chargeDistance * (puckPos - (Vector2)(_opponentGoal.position)).normalized;
        if(Vector2.Distance(target, thisTransform.position) < chargeDistance)
        {
            SetTargetVector(puckPos);
        }
        else
        {
            SetTargetVector(target);
        }
        action.aimingInputDirection = (_opponentGoal.position - thisTransform.position);
    }

    protected override void updateAim()
    {
        
    }
}
