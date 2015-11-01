using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
public class InputToAction : MonoBehaviour {

    Rigidbody2D rigid;
    MovementAbility moveAbility;
    GenericAbility genAbility;
    SuperAbility superAbility;
    Transform rotating;

    public Vector2 normalizedMovementInput { get; set; }
    bool _movementEnabled = false;
    public bool movementEnabled { get { return _movementEnabled; } set { _movementEnabled = value; } }
    bool _rotationEnabled = true;
    public bool rotationEnabled { get { return _rotationEnabled; } set { _rotationEnabled = value; } }
    public Vector2 aimingInputDirection { get; set; }
    public delegate Vector2 vectorQuantifier(Vector2 aimingInput, float maxDistance);
    vectorQuantifier _vectorQuantified;
    public vectorQuantifier vectorQuantified {set { _vectorQuantified = value; } }
    public delegate float vectorPercent(Vector2 aimingInput, float maxDistance);
    vectorPercent _vectorPercent;
    public vectorPercent vectorToPercent { set { _vectorPercent = value; } }
    public Vector2 aimingInputDisplacement(float maxDistanceForScale) { return _vectorQuantified(aimingInputDirection, maxDistanceForScale); }
    public float aimingInputPercentDistance(float maxDistanceForScale) { return _vectorPercent(aimingInputDirection, maxDistanceForScale); }
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

         rotating = transform.Find("Rotating");
	}

    void Start()
    {
        moveAbility = GetComponentInChildren<MovementAbility>();
        superAbility = GetComponentInChildren<SuperAbility>();
        genAbility = GetComponentInChildren<GenericAbility>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (_movementEnabled)
            rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, _maxSpeed * normalizedMovementInput, _maxSpeed * _accel * Time.fixedDeltaTime), _maxSpeed);

        //rotation
        if (aimingInputDirection.sqrMagnitude != 0)
            rotateTowards(aimingInputDirection);
        else if (normalizedMovementInput.sqrMagnitude != 0)
            rotateTowards(normalizedMovementInput);
	}

    void rotateTowards(Vector2 targetDirection)
    {
        if (_rotationEnabled)
        {
            rotating.rotation = Quaternion.Slerp(rotating.rotation, targetDirection.ToRotation(), rotationLerpValue); //it's in fixed update, and the direction property should be used instead of sampling the transform
            _direction = targetDirection;
        }
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

    public void DisableMovement(float duration)
    {
        _movementEnabled = false;
        rigid.velocity = Vector2.zero;
        Callback.FireAndForget(() => _movementEnabled = true, duration, this);
    }
}
