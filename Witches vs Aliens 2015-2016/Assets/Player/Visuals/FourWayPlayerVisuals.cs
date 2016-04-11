using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class FourWayPlayerVisuals : AbstractPlayerVisuals {

    [SerializeField]
    protected Sprite upSprite;
    [SerializeField]
    protected Sprite leftSprite;
    [SerializeField]
    protected Sprite rightSprite;
    [SerializeField]
    protected Sprite downSprite;

    public override Sprite selectionSprite(Vector2 visualSpaceInput) { return downSprite; }

    SpriteRenderer rend;
    Direction prevDirection;
	// Use this for initialization

    public override float alpha
    {
        get
        {
            return rend.color.a;
        }
        set
        {
            rend.color = rend.color.setAlphaFloat(value);
        }
    }

	void Awake () {
        rend = GetComponent<SpriteRenderer>();
	}

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        Direction newDirection = vectorToDirection(direction);
        //Debug.Log(newDirection);
        if(newDirection == prevDirection)
            return; //don't need to do anything

        switch (newDirection)
        {
            case Direction.UP:
                rend.sprite = upSprite;
                break;
            case Direction.LEFT:
                rend.sprite = leftSprite;
                break;
            case Direction.RIGHT:
                rend.sprite = rightSprite;
                break;
            case Direction.DOWN:
                rend.sprite = downSprite;
                break;

        }
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
