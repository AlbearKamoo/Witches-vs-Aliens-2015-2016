using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Boid : MonoBehaviour, IBoid, INetworkable, IObserver<OutgoingNetworkStreamMessage>
{

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float accel;

    [SerializeField]
    protected float seperationDistance;

    [SerializeField]
    protected float seperationWeight;
    [SerializeField]
    protected float cohesionWeight;
    [SerializeField]
    protected float alignmentWeight;
    [SerializeField]
    protected float randomWeight;
    [SerializeField]
    protected float staticAvoidWeight;
    [SerializeField]
    protected float mobileAvoidWeight;

    static Dictionary<int, Boid> boids = new Dictionary<int, Boid>();
    static int nextID;
    static int subscribedID;

    public float seperation { get { return seperationDistance; } }
    public Vector2 position { get { return transform.position; } }

    [SerializeField]
    protected float blindSpotAngle;

    Rigidbody2D rigid;
    public Vector2 velocity { get { return rigid.velocity; } }

    CircleCollider2D coll;
    public CircleCollider2D Coll { get { return coll; } }
    Collider2D trigger;

    Material vectoredMetaballMat;

    List<IBoid> neighbors = new List<IBoid>();
    List<IStaticAvoidBoid> staticAvoid = new List<IStaticAvoidBoid>();
    List<IMobileAvoidBoid> mobileAvoid = new List<IMobileAvoidBoid>();
    System.Random randGenerator;
    Transform visuals;
    Vector2 staticAvoidVelocity = Vector2.zero;

    int id;
    NetworkNode node;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        trigger = GetComponent<Collider2D>();
        visuals = transform.Find("visuals");
        coll = visuals.GetComponent<CircleCollider2D>();
        randGenerator = new System.Random(UnityEngine.Random.Range(0, 9999999));
        vectoredMetaballMat = transform.Find("metaBallVisuals").GetComponent<SpriteRenderer>().material;
        vectoredMetaballMat.SetFloat("_NumBoids", 8);

        node = NetworkNode.node;
        if (node != null && (node is Client || node is Server))
        {
            id = nextID++;
            
            if (node is Server)
            {
                node.Subscribe<OutgoingNetworkStreamMessage>(this);
            }
            else if (node is Client && boids.Count == 0)
            {
                Debug.Log("DING!");
                subscribedID = id;
                node.Subscribe(this);
            }

            boids[id] = this;
        }

    }

    void Start()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(this.transform.position, GetComponent<CircleCollider2D>().radius);
        for (int i = 0; i < others.Length; i++)
        {
            OnTriggerEnter2D(others[i]);
        }
        rigid.velocity = speed * Random.insideUnitCircle;
    }

    void OnDestroy()
    {
        if (node != null)
        {
            node.Unsubscribe<OutgoingNetworkStreamMessage>(this);
            boids.Remove(id);
            if (subscribedID == id && boids.Count != 0) //set up another boid to be the subscribed
            {
                node.Unsubscribe(this);
                subscribedID = boids.Keys.First();
                node.Subscribe(boids[subscribedID]);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IBoid otherBoid = other.GetComponentInParent<IBoid>();
        if (otherBoid != null && !neighbors.Contains(otherBoid) && otherBoid as Boid != this)
        {
            neighbors.Add(otherBoid);
        }

        IStaticAvoidBoid otherStaticAvoid = other.GetComponentInParent<IStaticAvoidBoid>();
        if (otherStaticAvoid != null && !staticAvoid.Contains(otherStaticAvoid))
        {
            staticAvoid.Add(otherStaticAvoid);
            UpdateStaticAvoidance();
        }

        IMobileAvoidBoid otheMobileAvoid = other.GetComponentInParent<IMobileAvoidBoid>();
        if (otheMobileAvoid != null && !mobileAvoid.Contains(otheMobileAvoid))
        {
            mobileAvoid.Add(otheMobileAvoid);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IBoid otherBoid = other.GetComponentInParent<IBoid>();
        if (otherBoid != null)
            neighbors.Remove(otherBoid);

        IStaticAvoidBoid otherStaticBoid = other.GetComponentInParent<IStaticAvoidBoid>();
        if (otherStaticBoid != null)
        {
            staticAvoid.Remove(otherStaticBoid);
            UpdateStaticAvoidance();
        }

        IMobileAvoidBoid otherMobileBoid = other.GetComponentInParent<IMobileAvoidBoid>();
        if (otherMobileBoid != null)
            mobileAvoid.Remove(otherMobileBoid);
    }

    public void ignoreCollision(Collider2D other)
    {
        Physics2D.IgnoreCollision(other, coll);
    }

    public void ignoreCollisions(IEnumerable<Collider2D> others)
    {
        foreach (Collider2D other in others)
        {
            ignoreCollision(other);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 alignVelocity = Vector2.zero;
        Vector2 center = Vector2.zero;
        int numNeighbors = 0; //don't include self
        Vector2 seperationForce = Vector2.zero;
        int seperationCount = 0;
        
        foreach (IBoid boid in neighbors)
        {
            if (!inBlindSpot(boid))
            {
                center += boid.position;
                alignVelocity += boid.velocity;
                numNeighbors++;
            }

            Vector2 displacement = this.position - boid.position;

            float distance = displacement.magnitude;
            if (distance < boid.seperation)
            {
                seperationForce += displacement / distance; //normalize, then weight by distance; (small distance = larger vector)
                seperationCount++;
            }
        }

        if (numNeighbors > 0)
        {
            center /= (float) numNeighbors;
            //turn center into a steering vector
            center = (center - (Vector2)this.position).normalized;
            center = DirectionToSteering(center);

            alignVelocity /= (float) numNeighbors;
            //turn velocity into a steering vector
            alignVelocity = DirectionToSteering(alignVelocity.normalized); 
        }
        //else vectors remain as Vector2.zero

        if (seperationCount > 0)
        {
            seperationForce = DirectionToSteering(seperationForce.normalized);
        }
        //else is Vector2.zero

        Vector2 steeringMobileAvoidingVelocity = Vector2.zero;
        int numMobileAvoids = 0;

        foreach (IMobileAvoidBoid boid in mobileAvoid)
        {
            Vector2 displacement = this.position - boid.position;
            float distance = displacement.magnitude;

            steeringMobileAvoidingVelocity += (displacement / distance) / (distance - coll.radius);
            float dot = Vector2.Dot(displacement, boid.velocity);
            if (distance > 0.25 && dot > 0) //if they have a chance of hitting us
            {
                Vector2 fleeDirection = displacement - (dot * boid.velocity / Vector2.Dot(boid.velocity, boid.velocity)); //vector component orthogonal to the velocity
                steeringMobileAvoidingVelocity += fleeDirection.normalized;
                numMobileAvoids++;
            }
        }

        

        if (numMobileAvoids > 0)
        {
            steeringMobileAvoidingVelocity /= (float)numMobileAvoids;
            steeringMobileAvoidingVelocity = DirectionToSteering(steeringMobileAvoidingVelocity);
        }

        Vector2 randomVector = ((float)(2 * Mathf.PI * randGenerator.NextDouble())).RadToVector2();
        randomVector = DirectionToSteering(randomVector);

        Vector2 steeringStaticAvoidingVelocity = DirectionToSteering(staticAvoidVelocity);

        //multiply steering by weights
        center *= cohesionWeight;
        alignVelocity *= alignmentWeight;
        seperationForce *= seperationWeight;
        randomVector *= randomWeight;
        steeringStaticAvoidingVelocity *= staticAvoidWeight;
        steeringMobileAvoidingVelocity *= mobileAvoidWeight;

        Vector2 newRigidbodyVelocity = rigid.velocity;
        newRigidbodyVelocity = Vector2.ClampMagnitude(newRigidbodyVelocity + center + alignVelocity + seperationForce + randomVector + steeringStaticAvoidingVelocity + steeringMobileAvoidingVelocity, speed);
        rigid.velocity = newRigidbodyVelocity;

        visuals.rotation = newRigidbodyVelocity.ToRotation();

        newRigidbodyVelocity = ((newRigidbodyVelocity / speed) + Vector2.one)/2; //map it to [0,1] {zero vector becomes (0.5, 0.5)}

        vectoredMetaballMat.SetFloat("_XVel", newRigidbodyVelocity.x);
        vectoredMetaballMat.SetFloat("_YVel", newRigidbodyVelocity.y);
	}

    bool inBlindSpot(IBoid boid)
    {
        return Vector2.Angle(rigid.velocity, boid.position - this.position) > blindSpotAngle;
    }

    Vector2 DirectionToSteering(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return Vector2.zero;
        else
        {
            return Vector2.ClampMagnitude(speed * direction - rigid.velocity, accel * Time.fixedDeltaTime);  //Reynolds: Steering = Desired - Velocity
        }
    }

    void UpdateStaticAvoidance()
    {
        staticAvoidVelocity = Vector2.zero;
        int numAvoids = 0;
        foreach (IStaticAvoidBoid boid in staticAvoid)
        {
            staticAvoidVelocity += boid.direction;
            numAvoids++;
        }

        if(numAvoids > 0)
            staticAvoidVelocity /= (float)numAvoids;
    }

    public PacketType[] packetTypes { get { return new PacketType[]{PacketType.BOIDPOSITION}; } }

    public void Notify(OutgoingNetworkStreamMessage m)
    {
        Assert.IsTrue(node.networkMode == NetworkMode.LOCALSERVER);
        {
            m.writer.Write((byte)(PacketType.BOIDPOSITION));
            m.writer.Write((byte)(id));
            m.writer.Write((Vector2)(this.transform.position));
            m.writer.Write(rigid.velocity);
        }
    }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        int index = m.reader.ReadByte();
        Assert.IsTrue(boids.ContainsKey(index));
        switch (m.packetType)
        {
            case PacketType.BOIDPOSITION:
                boids[index].rigid.position = m.reader.ReadVector2();
                boids[index].rigid.velocity = m.reader.ReadVector2();
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}

public interface IBoid
{
    float seperation { get; }
    Vector2 position { get; }
    Vector2 velocity { get; }
}

public interface IStaticAvoidBoid
{
    Vector2 direction { get; }
}

public interface IMobileAvoidBoid
{
    Vector2 position { get; }
    Vector2 velocity { get; }
    float radius { get; }
}