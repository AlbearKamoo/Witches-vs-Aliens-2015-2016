using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using System.IO;

[RequireComponent(typeof(Rigidbody2D))]
public class InputToAction : MonoBehaviour, ISpeedLimiter, INetworkable, IObserver<OutgoingNetworkStreamMessage>
{
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

    private FloatStatTracker _mass;
    public FloatStatTracker mass { get { return _mass; } }

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

    Rigidbody2D rigid;
    AbstractMovementAbility moveAbility;
    public AbstractMovementAbility MoveAbility { set { moveAbility = value; } }
    AbstractGenericAbility genAbility;
    public AbstractGenericAbility GenAbility { set { genAbility = value; } }
    SuperAbility superAbility;
    public SuperAbility SuperAbility { set { superAbility = value; } }

    NetworkNode node;
    Stats stats; //used only for playerID in networking

    class NetworkPlayerIdentity : IComparable<NetworkPlayerIdentity>
    {
        public readonly int playerID;
        public readonly NetworkMode networkMode;
        public readonly InputToAction inputToAction;
        public NetworkPlayerIdentity(int playerID, NetworkMode networkState, InputToAction inputToAction)
        {
            this.playerID = playerID;
            this.networkMode = networkState;
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
         _mass = new FloatStatTracker(rigid.mass, () => rigid.mass = _mass.value);
	}

    void Start()
    {
        moveAbility = GetComponentInChildren<AbstractMovementAbility>();
        superAbility = GetComponentInChildren<SuperAbility>();
        genAbility = GetComponentInChildren<AbstractGenericAbility>();

        stats = GetComponent<Stats>();

        node = NetworkNode.node;
        if (node is Client || node is Server)
        {
            if(players.Count == 0)
                node.Subscribe(this); //only the first element should be subscribed to the node
            players.Add(new NetworkPlayerIdentity(stats.playerID, stats.networkMode, this));
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
        else
            rigid.velocity = Vector2.ClampMagnitude(Vector2.MoveTowards(rigid.velocity, Vector2.zero, _maxSpeed * _accel * Time.fixedDeltaTime), _maxSpeed);

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
        switch (stats.networkMode)
        {
            case NetworkMode.LOCALCLIENT:
                switch (t)
                {
                    case AbilityType.MOVEMENT:
                        node.BinaryWriter.Write((byte)(PacketType.PLAYERMOVEMENTABILITY));
                        break;
                    case AbilityType.GENERIC:
                        node.BinaryWriter.Write((byte)(PacketType.PLAYERGENERICABILITY));
                        break;
                    case AbilityType.SUPER:
                    default:
                        node.BinaryWriter.Write((byte)(PacketType.PLAYERSUPERABILITY));
                        break;
                }
                node.BinaryWriter.Write((byte)(stats.playerID));
                node.BinaryWriter.Write(direction);
                node.Send(node.ConnectionIDs, node.AllCostChannel);
                break;
            //add ability.LocalFire, for immediate feedback awaiting server validation
            case NetworkMode.LOCALSERVER:
                switch (t)
                {
                    case AbilityType.MOVEMENT:
                        activateAndSend(PacketType.PLAYERMOVEMENTABILITY, direction, (byte)(stats.playerID), moveAbility);
                        break;
                    case AbilityType.GENERIC:
                        activateAndSend(PacketType.PLAYERGENERICABILITY, direction, (byte)(stats.playerID), genAbility);
                        break;
                    case AbilityType.SUPER:
                        activateAndSend(PacketType.PLAYERSUPERABILITY, direction, (byte)(stats.playerID), superAbility);
                        break;
                }
                
                break;
            default:
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

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.PLAYERLOCATION, PacketType.PLAYERINPUT, PacketType.PLAYERMOVEMENTABILITY, PacketType.PLAYERGENERICABILITY, PacketType.PLAYERSUPERABILITY }; } }

    public void Notify(OutgoingNetworkStreamMessage m)
    {
        switch (stats.networkMode)
        {
            case NetworkMode.LOCALSERVER:
            case NetworkMode.REMOTESERVER:
                m.writer.Write((byte)(PacketType.PLAYERLOCATION));
                m.writer.Write((byte)(stats.playerID));
                m.writer.Write((Vector2)(this.transform.position));
                m.writer.Write(rigid.velocity);
                break;
            case NetworkMode.LOCALCLIENT:
                m.writer.Write((byte)(PacketType.PLAYERINPUT));
                m.writer.Write((byte)(stats.playerID));
                m.writer.Write((Vector2)(normalizedMovementInput));
                break;
        }
        
    }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        int index = findPlayerIndex(m.reader);
        switch (m.packetType)
        {  
            case PacketType.PLAYERLOCATION:
                if (checkValidPlayer(m, index, 2))
                {
                    players[index].inputToAction.transform.position = m.reader.ReadVector2();
                    Vector2 velocity = m.reader.ReadVector2();
                    players[index].inputToAction.rigid.velocity = velocity;
                    if (players[index].networkMode == NetworkMode.REMOTECLIENT)
                    {
                        players[index].inputToAction.normalizedMovementInput = velocity.normalized;
                    }
                }
                break;
            case PacketType.PLAYERINPUT:
                if (checkValidPlayer(m, index))
                {
                    Assert.IsTrue(players[index].networkMode == NetworkMode.REMOTESERVER);
                    players[index].inputToAction.normalizedMovementInput = m.reader.ReadVector2();
                }
                break;

            case PacketType.PLAYERMOVEMENTABILITY:
                if (checkValidPlayer(m, index))
                {
                    handleNetworkedAbilityInput(m, index, players[index].inputToAction.moveAbility);
                }
                break;
            case PacketType.PLAYERGENERICABILITY:
                if (checkValidPlayer(m, index))
                {
                    handleNetworkedAbilityInput(m, index, players[index].inputToAction.genAbility);
                }
                break;
            case PacketType.PLAYERSUPERABILITY:
                if (checkValidPlayer(m, index))
                {
                    handleNetworkedAbilityInput(m, index, players[index].inputToAction.superAbility);
                }
                break;
                
                
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }

    void handleNetworkedAbilityInput(IncomingNetworkStreamMessage m, int index, AbstractAbility ability)
    {
        switch (players[index].networkMode)
        {
            case NetworkMode.REMOTESERVER:
                {
                    Vector2 direction = m.reader.ReadVector2();
                    activateAndSend(m.packetType, direction, (byte)(players[index].playerID), ability);
                    //no response when failed; client uses RTT to realize this
                    break;
                }
            case NetworkMode.LOCALCLIENT:
            case NetworkMode.REMOTECLIENT:
                {
                    Vector2 direction = m.reader.ReadVector2();
                    bool activated;
                    if (m.reader.ReadBoolean()) //if it is a IRandomAbility
                    {
                        activated = ((IRandomAbility)ability).Fire(direction, m.reader.ReadInt32());
                    }
                    else
                    {
                        activated = ability.Fire(direction);
                    }
                    //maybe call a ForceFire method?
                    Assert.IsTrue(activated);
                    break;
                }
            default:
                Debug.Log("Unauthorized ability input");
                break;
        }
    }

    void activateAndSend(PacketType packetType, Vector2 direction, byte playerID, AbstractAbility ability)
    {
        int seed = -1;
        bool activated;
        if (ability is IRandomAbility)
        {
            seed = RandomLib.Seed();
            activated = ((IRandomAbility)ability).Fire(direction, seed);
        }
        else
        {
            activated = ability.Fire(direction);
        }

        if (activated)
        {
            node.BinaryWriter.Write((byte)(packetType));
            node.BinaryWriter.Write(playerID);
            node.BinaryWriter.Write(direction);
            if (ability is IRandomAbility)
            {
                node.BinaryWriter.Write(true);
                node.BinaryWriter.Write(seed);
            }
            else
            {
                node.BinaryWriter.Write(false);
            }
            node.Send(node.ConnectionIDs, node.AllCostChannel);
        }
    }

    bool checkValidPlayer(IncomingNetworkStreamMessage m, int index, int numVector2s = 1)
    {
        if (index < 0)
        {
            Debug.Log("Unknown Player");
            for (int i = 0; i < numVector2s; i++ )
                m.reader.ReadVector2(); //move the stream for the next data element
            return false;
        }
        return true;
    }

    static int findPlayerIndex(int playerID)
    {
        return players.BinarySearch(new NetworkPlayerIdentity(playerID, NetworkMode.UNKNOWN, null));
    }

    static int findPlayerIndex(BinaryReader reader)
    {
        return findPlayerIndex(reader.ReadByte());
    }

    void OnDestroy()
    {
        int myIndex = findPlayerIndex(stats.playerID);
        if(myIndex != -1)
        {
            players.RemoveAt(myIndex);
            if (node != null && myIndex == 0 && players.Count != 0)
            {
                node.Subscribe(players[0].inputToAction); //have someone else subscribe to the networked node
            }
        }
        if (node != null)
            node.Unsubscribe<OutgoingNetworkStreamMessage>(this);
    }
    /// <summary>
    /// Only supposed to be used when networking playerIDs. If you didn't get the ID from networking, find the InputToAction the same way you got the ID.
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    public static InputToAction IDToInputToAction(int playerID)
    {
        int index = findPlayerIndex(playerID);
        if (index < 0)
            return null;
        else
            return players[index].inputToAction;
    }
}