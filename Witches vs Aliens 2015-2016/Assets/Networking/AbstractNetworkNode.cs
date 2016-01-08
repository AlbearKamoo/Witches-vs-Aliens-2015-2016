using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

public abstract class AbstractNetworkNode : MonoBehaviour {
    [SerializeField]
    protected float packetPeriod;

    public abstract bool active { get; }
    //contains the non-project-specific stuff from network node

    public static AbstractNetworkNode self; //there can only be one

    protected List<int> connectionIDs = new List<int>();

    protected int hostID = -1;

    Dictionary<PacketType, IObserver<IncomingNetworkStreamReaderMessage>> identities = new Dictionary<PacketType, IObserver<IncomingNetworkStreamReaderMessage>>();
    //each int is a different message type. If there are multiple possible destinations (i.e. syncing player positions), the IObserver's Observe method will use statics to determine the proper recipient

    protected virtual void Awake()
    {
        self = this;
    }

    protected virtual void Start()
    {
        Application.runInBackground = true;
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        ConfigureChannels();
        ConfigureHosts();
    }

    protected abstract void ConfigureChannels();
    protected abstract void ConfigureHosts();

    public void Subscribe(IObserver<IncomingNetworkStreamReaderMessage> observer, params PacketType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
            identities[types[i]] = observer;
        }
    }

    protected void ProcessNetworkData(byte[] buffer)
    {
        MemoryStream stream = new MemoryStream(buffer);
        BinaryReader reader = new BinaryReader(stream);

        while (stream.Position != buffer.Length)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            identities[packetType].Notify(new IncomingNetworkStreamReaderMessage(reader, packetType));
        }
    }

    
}

public class OutgoingNetworkStreamReaderMessage
{
    readonly BinaryWriter writer; //observers write their set of data (format will vary) in order, starting with a PacketType (the enum) as a byte

    public OutgoingNetworkStreamReaderMessage(BinaryWriter writer)
    {
        this.writer = writer;
    }
}

public class IncomingNetworkStreamReaderMessage
{
    readonly BinaryReader reader; //observers read their set of data (format will vary), which moves the stream to the next set of data
    readonly PacketType packetType;

    public IncomingNetworkStreamReaderMessage(BinaryReader reader, PacketType packetType)
    {
        this.reader = reader;
        this.packetType = packetType;
    }
}