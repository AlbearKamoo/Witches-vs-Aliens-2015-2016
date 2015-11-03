using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InterposeAI : AbstractAI, InterferenceAI
{
    List<Transform> opponents;
    public List<Transform> myOpponents { set { opponents = value; } }

    const float guardDistance = 1f;

    Transform targetTransform;

    protected override void updateMovement()
    {
        if (opponents.Count != 0)
        {
            Vector2 puckPos = puckTransform.position;
            targetTransform = opponents[0];

            float distanceSquared = (((Vector2)(targetTransform.position)) - puckPos).sqrMagnitude;
            for (int i = 1; i < opponents.Count; i++)
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
                if (Vector2.Distance(nearestOnLinePoint, puckPos) < guardDistance)
                    nearestOnLinePoint = guardDistance * (((Vector2)(targetTransform.position)) - puckPos).normalized;
                SetTargetVector(nearestOnLinePoint); return;
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

    protected override void updateAim()
    {
        action.aimingInputDirection = (puckTransform.position - thisTransform.position);
    }
}
