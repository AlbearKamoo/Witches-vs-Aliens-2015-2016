using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class PuckSpeedLimiter : MonoBehaviour, ISpeedLimiter, INetworkable, IObserver<OutgoingNetworkStreamMessage>
{
    [SerializeField]
    protected float initialMaxSpeed;

    [SerializeField]
    protected float dragCutoff;

    [SerializeField]
    protected float dragSpeedMultiplier;

    [SerializeField]
    protected float dragMin = 0.25f;
    Rigidbody2D rigid;
    NetworkNode node;

    public float maxSpeed { get; set; }
    // Use this for initialization
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        maxSpeed = initialMaxSpeed;
    }

    void Start()
    {
        node = NetworkNode.node;
        if (node is Client)
        {
            node.Subscribe(this);
        }
        else if (node is Server)
        {
            node.Subscribe<OutgoingNetworkStreamMessage>(this);
        }
        updateDrag();
    }

    // Update is called once per frame
    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
        updateDrag();
    }

    void FixedUpdate()
    {
        updateDrag();
    }

    void updateDrag()
    {
        rigid.drag = dragMin + (dragSpeedMultiplier * Mathf.Max(0, rigid.velocity.magnitude - dragCutoff));
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.PUCKLOCATION }; } }

    public void Notify(OutgoingNetworkStreamMessage m)
    {
        m.writer.Write((byte)(PacketType.PUCKLOCATION));
        m.writer.Write((Vector2)(this.transform.position));
        m.writer.Write(rigid.velocity);
    }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.PUCKLOCATION:
                this.transform.position = m.reader.ReadVector2();
                rigid.velocity = m.reader.ReadVector2();
                updateDrag();
                break;
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }
}

public interface ISpeedLimiter
{
    float maxSpeed { get; }
}