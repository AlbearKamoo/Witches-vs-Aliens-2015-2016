using UnityEngine;
using System.Collections;
using System.Linq;
public class DefensiveAI : AbstractPlayerInput
{
    Transform puckTransform;
    Transform thisTransform;
    Transform goalTransform;

    const float chargeDistance = 0.75f;

    void Awake()
    {
        puckTransform = GameObject.FindGameObjectWithTag(Tags.puck).transform;
        thisTransform = this.transform;
        Side mySide = GetComponent<Stats>().side;
        goalTransform = GameObject.FindObjectsOfType<Goal>().Where((Goal g) => g.side == mySide).ToArray<Goal>()[0].transform;
    }

    protected override void setInputToActionAimingDelegates()
    {
        action.vectorQuantified = (Vector2 aim, float distance) => Vector2.ClampMagnitude(aim, distance);
        action.vectorToPercent = (Vector2 aim, float distance) => Mathf.Clamp01(aim.magnitude / distance);
    }

    protected override void updateMovement()
    {
        Vector2 puckPos = puckTransform.position;

        Vector2 nearestOnLinePoint = ClosestPointOnLine(goalTransform.position, puckPos, thisTransform.position);
        if (OnLine(goalTransform.position, puckPos, nearestOnLinePoint))
        {
            if (Vector2.Distance(nearestOnLinePoint, thisTransform.position) < chargeDistance)
                SetTargetVector(puckPos);
            else
                SetTargetVector(nearestOnLinePoint);
        }
        else
        {
            SetTargetVector(goalTransform.position);
        }
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
