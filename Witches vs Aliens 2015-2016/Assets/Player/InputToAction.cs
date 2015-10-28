using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VisualAnimate))]
public class InputToAction : MonoBehaviour {

    Rigidbody2D rigid;
    MovementAbility moveAbility;
    GenericAbility genAbility;
    SuperAbility superAbility;
    VisualAnimate vfx;
    Transform rotating;

    public Vector2 normalizedMovementInput { get; set; }
    public Vector2 aimingInput { get; set; }
    [SerializeField]
    protected float initMaxSpeed;
    private FloatStatTracker _maxSpeed;
    public FloatStatTracker maxSpeed { get { return _maxSpeed; } }

    [SerializeField]
    protected float initAccel;
    private FloatStatTracker _accel;
    public FloatStatTracker accel { get { return _accel; } }

    [Range(0,1)]
    public float rotationLerpValue;

    Vector2 _direction = Vector2.zero;
    public Vector2 direction { get {
        return _direction;
    } }

    //Don't change the rigidbody mass from 1 to change speed/agility; change accel and maxSpeed instead
    //the rigidbody mass(es) generally only affect how collisions happen
	// Use this for initialization
	void Awake () {
         rigid = GetComponent<Rigidbody2D>();
         _maxSpeed = new FloatStatTracker(initMaxSpeed);
         _accel = new FloatStatTracker(initAccel);

         vfx = GetComponent<VisualAnimate>();

         rotating = transform.Find("Rotating");
	}

    void Start()
    {
        moveAbility = GetComponentInChildren<MovementAbility>();
        superAbility = GetComponentInChildren<SuperAbility>();
        genAbility = GetComponentInChildren<GenericAbility>();
        vfx.DoFX();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, _maxSpeed * normalizedMovementInput, _maxSpeed * _accel * Time.fixedDeltaTime), _maxSpeed);

        //rotation
        if (aimingInput.sqrMagnitude != 0)
            rotateTowards(aimingInput);
        else if (normalizedMovementInput.sqrMagnitude != 0)
            rotateTowards(normalizedMovementInput);
	}

    void rotateTowards(Vector2 targetDirection)
    {
        rotating.rotation = Quaternion.Slerp(rotating.rotation, targetDirection.ToRotation(), rotationLerpValue); //it's in fixed update, and the direction property should be used instead of sampling the transform
        _direction = targetDirection;
    }

    public void FireAbility(AbilityType t)
    {
        switch (t)
        {
            case AbilityType.MOVEMENT:
                moveAbility.Fire(direction);
                break;
            case AbilityType.GENERIC:
                genAbility.Fire(direction);
                break;
            case AbilityType.SUPER:
                superAbility.Fire(direction);
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
            case AbilityType.GENERIC:
                genAbility.StopFire();
                break;
            case AbilityType.SUPER:
                superAbility.StopFire();
                break;
        }
    }
}
