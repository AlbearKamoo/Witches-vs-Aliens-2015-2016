using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VisualAnimate))]
public class InputToAction : MonoBehaviour {

    Rigidbody2D rigid;
    MovementAbility moveAbility;
    SuperAbility superAbility;
    VisualAnimate vfx;

    public Vector2 normalizedInput { get; set; }
    [SerializeField]
    protected float initMaxSpeed;
    private FloatStatTracker _maxSpeed;
    public FloatStatTracker maxSpeed { get { return _maxSpeed; } }

    public float accel;
    private float scaledAccel; //scales the acceleration to maxSpeed; what we actually use for stuff.
    private void updateScaledAccel()
    {
        scaledAccel = _maxSpeed * accel;
    }

    //Don't change the rigidbody mass from 1 to change speed/agility; change accel and maxSpeed instead
    //the rigidbody mass(es) generally only affect how collisions happen
	// Use this for initialization
	void Awake () {
         rigid = GetComponent<Rigidbody2D>();
         _maxSpeed = new FloatStatTracker(initMaxSpeed, updateScaledAccel);
         updateScaledAccel();

         vfx = GetComponent<VisualAnimate>();
	}

    void Start()
    {
        moveAbility = GetComponentInChildren<MovementAbility>();
        superAbility = GetComponentInChildren<SuperAbility>();
        vfx.DoFX();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, _maxSpeed * normalizedInput, scaledAccel * Time.fixedDeltaTime), _maxSpeed);
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
