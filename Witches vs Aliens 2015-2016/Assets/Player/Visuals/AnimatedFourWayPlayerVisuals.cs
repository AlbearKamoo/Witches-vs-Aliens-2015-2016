using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedFourWayPlayerVisuals : AbstractPlayerVisuals
{

	[SerializeField]
    protected Sprite[] upSprites;
    [SerializeField]
    protected Sprite[] leftSprites;
    [SerializeField]
    protected Sprite[] rightSprites;
    [SerializeField]
    protected Sprite[] downSprites;

    [SerializeField]
    protected float timePerLoop;

    public override Sprite selectionSprite(Vector2 visualSpaceInput) { Assert.IsTrue(downSprites.Length > 0); return downSprites[0]; }
    SpriteRenderer rend;
    Direction prevDirection;

    float currentAnimationTime;
    float cycleTime;
    int currentFrame;
    Sprite[] currentSpritesheet;

	// Use this for initialization
	void Awake () {
        rend = GetComponent<SpriteRenderer>();
        restartAnimation();
	}

    protected override void UpdateVisualRotation(Vector2 direction)
    {
        Direction newDirection = vectorToDirection(direction);
        //Debug.Log(newDirection);
        if(newDirection != prevDirection)
        {
            prevDirection = newDirection;
            restartAnimation();
        }
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

    void restartAnimation()
    {
        currentAnimationTime = 0;
        currentFrame = 0;
        switch(prevDirection)
        {
            case Direction.UP:
                currentSpritesheet = upSprites;
                break;
            case Direction.LEFT:
                currentSpritesheet = leftSprites;
                break;
            case Direction.RIGHT:
                currentSpritesheet = rightSprites;
                break;
            case Direction.DOWN:
                currentSpritesheet = downSprites;
                break;
        }
        cycleTime = timePerLoop / currentSpritesheet.Length;
        updateSprite();
    }

    protected override void Update()
    {
        base.Update();
        currentAnimationTime += Time.deltaTime;
        if(currentAnimationTime > (currentFrame + 1) * cycleTime)
        {
            currentFrame = (currentFrame + 1);
            if(currentFrame >= currentSpritesheet.Length)
            {
                currentFrame = 0;
                currentAnimationTime -= timePerLoop;
            }
            updateSprite();
        }
    }

    void updateSprite()
    {
        rend.sprite = currentSpritesheet[currentFrame];
    }

    private enum Direction
    {
        UP,
        LEFT,
        RIGHT,
        DOWN
    }
}
