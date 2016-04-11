using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractAI : AbstractPlayerInput {

    protected Transform puckTransform;
    protected Transform thisTransform;

    protected virtual void Awake()
    {
        puckTransform = GameObject.FindGameObjectWithTag(Tags.puck).transform;
        thisTransform = this.transform;
    }

    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => Vector2.ClampMagnitude(aim, distance);
        action.vectorToPercent = (Vector2 aim, float distance) => Mathf.Clamp01(aim.magnitude / distance);
    }

    protected void SetTargetVector(Vector2 target)
    {
        action.normalizedMovementInput = (target - ((Vector2)(thisTransform.position))).normalized;
    }

    protected Vector2 ClosestPointOnLine(Vector2 linepoint1, Vector2 linepoint2, Vector2 point)
    {
        Vector2 pointMinusLine1 = point - linepoint1;
        Vector2 lineDisplacement = linepoint2 - linepoint1;
        float sqrdistance = lineDisplacement.sqrMagnitude;
        float distanceNormalized = Vector2.Dot(pointMinusLine1, lineDisplacement) / sqrdistance;
        return new Vector2(linepoint1.x + lineDisplacement.x * distanceNormalized, linepoint1.y + lineDisplacement.y * distanceNormalized);
    }

    protected bool OnLine(Vector2 linepoint1, Vector2 linepoint2, Vector2 point)
    {
        if (linepoint1.x < linepoint2.x)
        {
            if (point.x < linepoint1.x || point.x > linepoint2.x)
                return false;
        }
        else
        {
            if (point.x < linepoint2.x || point.x > linepoint1.x)
                return false;
        }

        if (linepoint1.y < linepoint2.y)
        {
            if (point.y < linepoint1.y || point.y > linepoint2.y)
                return false;
        }
        else
        {
            if (point.y < linepoint2.y || point.y > linepoint1.y)
                return false;
        }
        return true;
    }

    protected override void checkAbilities()
    {
        //ablities will be called in the steering behaviour
    }

    public override bool pressedAccept()
    {
        return false;
    }

    public override bool pressedBack()
    {
        return false;
    }

    public override Vector2 deltaVisuals()
    {
        return Vector2.zero;
    }
}

public interface IGoalAI
{
    Transform myGoal {set;}
    Transform opponentGoal {set;}
}

public interface IInterferenceAI
{
    List<Transform> myOpponents {set;}
}