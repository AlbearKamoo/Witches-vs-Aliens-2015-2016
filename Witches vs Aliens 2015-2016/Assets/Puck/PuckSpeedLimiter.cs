using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class PuckSpeedLimiter : MonoBehaviour, ISpeedLimiter, INetworkable
{
    [SerializeField]
    protected float initialMaxSpeed;
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
        node = GameObjectExtension.GetComponentWithTag<NetworkNode>(Tags.gameController);
        Debug.Log(node);
        if (node is Client)
        {
            Debug.Log("Subscribed to Client");
            node.Subscribe(this, packetTypes);
        }
        else if (node is Server)
        {
            Debug.Log("Subscribed to Server");
            node.Subscribe<OutgoingNetworkStreamReaderMessage>(this);
        }
    }

    // Update is called once per frame
    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.PUCKLOCATION }; } }

    public void Notify(OutgoingNetworkStreamReaderMessage m)
    {
        Debug.Log("writing data");
        m.writer.Write((byte)(PacketType.PUCKLOCATION));
        m.writer.Write((Vector2)(this.transform.position));
        m.writer.Write(rigid.velocity);
    }

    public void Notify(IncomingNetworkStreamReaderMessage m)
    {
        switch (m.packetType)
        {
            case PacketType.PUCKLOCATION:
                Debug.Log("reading data");
                this.transform.position = m.reader.ReadVector2();
                rigid.velocity = m.reader.ReadVector2();
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