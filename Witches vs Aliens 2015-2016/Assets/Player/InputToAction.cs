using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class InputToAction : MonoBehaviour, ISpeedLimiter, INetworkable, IObserver<OutgoingNetworkStreamMessage>
{

    Rigidbody2D rigid;
    MovementAbility moveAbility;
    public MovementAbility MoveAbility { set { moveAbility = value; } }
    GenericAbility genAbility;
    public GenericAbility GenAbility { set { genAbility = value; } }
    SuperAbility superAbility;
    public SuperAbility SuperAbility { set { superAbility = value; } }

    public Vector2 normalizedMovementInput { get; set; }
    bool _movementEnabled = false;
    public bool movementEnabled { get { return _movementEnabled; } set { _movementEnabled = value; if (!_movementEnabled) rigid.velocity = Vector3.zero; } }
    bool _rotationEnabled = true;
    public bool rotationEnabled { get { return _rotationEnabled; } set { _rotationEnabled = value; } }
    Vector2 _aimingInputDirection;
    public Vector2 aimingInputDirection { get { return _aimingInputDirection; } set { _aimingInputDirection = value; if(value.sqrMagnitude != 0) rotateTowards(_aimingInputDirection); } }
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
    public FloatStatTracker maxSpeedTracker { get { return _maxSpeed; } }
    public float maxSpeed { get { return _maxSpeed.value; } }

    [SerializeField]
    protected float initAccel;
    private FloatStatTracker _accel;
    public FloatStatTracker accel { get { return _accel; } }

    [Range(0,1)]
    public float rotationLerpValue;
    
    [SerializeField]
    [AutoLink(childPath = "Rotating")]
    protected Transform rotating;

    Vector2 _direction = Vector2.zero;
    public Vector2 direction { get {
        return _direction;
    } }

    private List<Callback.CallbackMethod> preFixedUpdateDelegates = new List<Callback.CallbackMethod>();
    public List<Callback.CallbackMethod> PreFixedUpdateDelegates { get { return preFixedUpdateDelegates; } }

    NetworkNode node;
    Stats stats; //used only for playerID in networking

    class NetworkPlayerIdentity : IComparable<NetworkPlayerIdentity>
    {
        public readonly int playerID;
        public readonly NetworkMode networkState;
        public readonly InputToAction inputToAction;
        public NetworkPlayerIdentity(int playerID, NetworkMode networkState, InputToAction inputToAction)
        {
            this.playerID = playerID;
            this.networkState = networkState;
            this.inputToAction = inputToAction;
        }
        public int CompareTo(NetworkPlayerIdentity other)
        {
            return this.playerID.CompareTo(other.playerID);
        }
    }

    //Static collection of all players for networking
    static List<NetworkPlayerIdentity> players = new List<NetworkPlayerIdentity>();
    //the first element should be subscribed to the network node for incoming messages, then use this static collection to direct it to the correct script

    //Don't change the rigidbody mass from 1 to change speed/agility; change accel and maxSpeed instead
    //the rigidbody mass(es) generally only affect how collisions happen
	// Use this for initialization
	void Awake () {
         rigid = GetComponent<Rigidbody2D>();
         _maxSpeed = new FloatStatTracker(initMaxSpeed);
         _accel = new FloatStatTracker(initAccel);
	}

    void Start()
    {
        moveAbility = GetComponentInChildren<MovementAbility>();
        superAbility = GetComponentInChildren<SuperAbility>();
        genAbility = GetComponentInChildren<GenericAbility>();

        stats = GetComponent<Stats>();

        node = GameObjectExtension.GetComponentWithTag<NetworkNode>(Tags.gameController);
        if (node is Client)
        {
            if(players.Count == 0)
                node.Subscribe(this); //only the first element should be subscribed to the node
            players.Add(new NetworkPlayerIdentity(stats.playerID, stats.networkState, this));
            
        }
        else if (node is Server)
        {
            node.Subscribe<OutgoingNetworkStreamMessage>(this);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        for (int i = 0; i < preFixedUpdateDelegates.Count; i++)
        {
            preFixedUpdateDelegates[i]();
        }
        if (_movementEnabled)
            rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, _maxSpeed * normalizedMovementInput, _maxSpeed * _accel * Time.fixedDeltaTime), _maxSpeed);

        //rotation
        if (aimingInputDirection.sqrMagnitude == 0 && normalizedMovementInput.sqrMagnitude != 0) //aimingInput rotation is handled when the aiming input is set
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

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.PLAYERLOCATION }; } }

    public void Notify(OutgoingNetworkStreamMessage m)
    {
        m.writer.Write((byte)(PacketType.PLAYERLOCATION));
        m.writer.Write((byte)(stats.playerID));
        m.writer.Write((Vector2)(this.transform.position));
        m.writer.Write(rigid.velocity);
    }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.PLAYERLOCATION:
                Debug.Log("reading data");
                int playerID = m.reader.ReadByte();
                int index = players.BinarySearch(new NetworkPlayerIdentity(playerID, NetworkMode.UNKNOWN, null));
                if (index < 0)
                {
                    Debug.Log("Unknown Player");
                    m.reader.ReadVector2(); m.reader.ReadVector2(); //move the stream for the next data element
                }
                players[index].inputToAction.transform.position = m.reader.ReadVector2();
                Vector2 velocity = m.reader.ReadVector2();
                players[index].inputToAction.rigid.velocity = velocity;
                Debug.Log(players[index].networkState);
                if (players[index].networkState == NetworkMode.REMOTECLIENT)
                {
                    players[index].inputToAction.normalizedMovementInput = velocity.normalized;
                    Debug.Log(players[index].inputToAction.normalizedMovementInput);
                }
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}
