using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(MovementAbility))]
[RequireComponent(typeof(SuperAbility))]
public class InputToAction : MonoBehaviour {

    Rigidbody2D rigid;
    MovementAbility moveAbility;
    SuperAbility superAbility;

    public Vector2 normalizedInput { get; set; }
    [SerializeField]
    protected float initMaxSpeed;
    private float maxSpeed;

    private List<FloatStat> speedModifiers = new List<FloatStat>();

    public float accel;
    private float scaledAccel; //scales the acceleration to maxSpeed; what we actually use for stuff.
    private void updateSpeed()
    {
        maxSpeed = initMaxSpeed;
        foreach(float i in speedModifiers)//recalculateing every removal prevents floating point madness/errors
            maxSpeed *= i;
        scaledAccel = maxSpeed * accel;
    }

    //Don't change the rigidbody mass from 1; change accel and maxSpeed instead
	// Use this for initialization
	void Awake () {
         rigid = GetComponent<Rigidbody2D>();
         moveAbility = GetComponent<MovementAbility>();
         superAbility = GetComponent<SuperAbility>();
         maxSpeed = initMaxSpeed;
         scaledAccel = maxSpeed * accel;
	}

    //refactor the modifer stuff into it's own class if we need to use this again
    public FloatStat addSpeedModifier(float value)
    {
        FloatStat result = new FloatStat(value, updateSpeed);
        speedModifiers.Add(result);
        updateSpeed();
        return result;
    }
    public void removeSpeedModifier(FloatStat modifier)
    {
        speedModifiers.Remove(modifier);
        updateSpeed();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, maxSpeed * normalizedInput, scaledAccel * Time.fixedDeltaTime), maxSpeed);
	}

    public void FireAbility(AbilityType t)
    {
        switch (t)
        {
            case AbilityType.MOVEMENT:
                moveAbility.Fire();
                break;
            case AbilityType.SUPER:
                superAbility.Fire();
                break;
        }
    }

    public void StopFireAbility(AbilityType t)
    {
        switch (t)
        {
            case AbilityType.MOVEMENT:
                moveAbility.StopFire();
                break;
            case AbilityType.SUPER:
                superAbility.StopFire();
                break;
        }
    }
}
