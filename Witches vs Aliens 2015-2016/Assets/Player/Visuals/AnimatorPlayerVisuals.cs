using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class AnimatorPlayerVisuals : AbstractPlayerVisuals
{
    Animator anim;
    Direction prevDirection;
    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        Direction newDirection = vectorToDirection(direction);
        //Debug.Log(newDirection);
        if (newDirection == prevDirection)
            return; //don't need to do anything

        anim.SetInteger("Direction", (int)newDirection);
        prevDirection = newDirection;
    }

    Direction vectorToDirection(Vector2 vector)
    {
        float angle = vector.ToAngle() - 45f;
        if (angle > 90f)
            return Direction.LEFT;
        else if (angle > 0f)
            return Direction.UP;
        else if (angle > -90f)
            return Direction.RIGHT;
        else
            return Direction.DOWN;
    }

    private enum Direction
    {
        UP,
        LEFT,
        RIGHT,
        DOWN
    }
}