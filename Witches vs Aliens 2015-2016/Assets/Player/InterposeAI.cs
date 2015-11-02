using UnityEngine;
using System.Collections;

public class InterposeAI : AbstractPlayerInput
{
    Transform puckTransform;
    Transform thisTransform;

    public Transform[] opponents = new Transform[0];

    Transform targetTransform;
    void Awake()
    {
        puckTransform = GameObject.FindGameObjectWithTag(Tags.puck).transform;
        thisTransform = this.transform;
    }

    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => Vector2.ClampMagnitude(aim, distance);
        action.vectorToPercent = (Vector2 aim, float distance) => Mathf.Clamp01(aim.magnitude / distance);
    }

    protected override void updateMovement()
    {
        if (opponents.Length != 0)
        {
            Vector2 puckPos = puckTransform.position;
            targetTransform = opponents[0];

            float distanceSquared = (((Vector2)(targetTransform.position)) - puckPos).sqrMagnitude;
            for (int i = 1; i < opponents.Length; i++)
            {
                float possibleNewDistance = (((Vector2)(opponents[i].position)) - puckPos).sqrMagnitude;
                if (possibleNewDistance < distanceSquared)
                {
                    distanceSquared = possibleNewDistance;
                    targetTransform = opponents[i];
                }
            }

            Vector2 nearestOnLinePoint = ClosestPointOnLine(targetTransform.position, puckPos, thisTransform.position);
            if (OnLine(targetTransform.position, puckPos, nearestOnLinePoint))
            {
                SetTargetVector((((Vector2)(targetTransform.position)) + puckPos)/2); return;
            }

            if ((targetTransform.position - thisTransform.position).sqrMagnitude < (puckTransform.position - thisTransform.position).sqrMagnitude)
            {
                SetTargetVector(targetTransform.position); return;
            }
        }
        //if all else fails, target the puck

        //consider using abilities here
        SetTargetVector(puckTransform.position);
    }
    void SetTargetVector(Vector2 target)
    {
        action.normalizedMovementInput = (target - ((Vector2)(thisTransform.position))).normalized;
    }

    Vector2 ClosestPointOnLine(Vector2 linepoint1, Vector2 linepoint2, Vector2 point)
    {
        Vector2 pointMinusLine1 = point - linepoint1;
        Vector2 lineDisplacement = linepoint2 - linepoint1;
        float sqrdistance = lineDisplacement.sqrMagnitude;
        float distanceNormalized = Vector2.Dot(pointMinusLine1, lineDisplacement) / sqrdistance;
        return new Vector2(linepoint1.x + lineDisplacement.x * distanceNormalized, linepoint1.y + lineDisplacement.y * distanceNormalized);
    }

    bool OnLine(Vector2 linepoint1, Vector2 linepoint2, Vector2 point)
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

    protected override void updateAim()
    {
        action.aimingInputDirection = (puckTransform.position - thisTransform.position);
    }

    protected override void checkAbilities()
    {

    }
}
