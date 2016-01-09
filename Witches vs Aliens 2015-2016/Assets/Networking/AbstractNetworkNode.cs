using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using System;

public abstract class AbstractNetworkNode : MonoBehaviour {
    [SerializeField]
    protected float packetPeriod;

    public bool active { get { return connectionIDs.Count != 0; } }
    //contains the non-project-specific stuff from network node

    public static AbstractNetworkNode self; //there can only be one

    protected HashSet<int> connectionIDs = new HashSet<int>();

    protected int hostID = -1;

    Dictionary<PacketType, IObserver<IncomingNetworkStreamReaderMessage>> Networkidentities = new Dictionary<PacketType, IObserver<IncomingNetworkStreamReaderMessage>>();
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
        ConfigureChannels(config);
        ConfigureHosts(config);
    }

    protected abstract void ConfigureChannels(ConnectionConfig config);
    protected abstract void ConfigureHosts(ConnectionConfig config);

    public void Subscribe(IObserver<IncomingNetworkStreamReaderMessage> observer, params PacketType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
            Networkidentities[types[i]] = observer;
        }
    }

    protected virtual void Update()
    {
        if (hostID == -1)
            return;
        int connectionID;
        int channelID;
        int receivedSize;
        byte error;
        byte[] buffer = new byte[1500];
        NetworkEventType networkEvent = NetworkTransport.ReceiveFromHost(hostID, out connectionID, out channelID, buffer, buffer.Length, out receivedSize, out error);
        switch (networkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                OnConnection(connectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                OnDisconnection(connectionID);
                break;
            case NetworkEventType.DataEvent:
                Debug.Log(string.Format("Got data size {0}", receivedSize));
                Array.Resize(ref buffer, receivedSize);
                ProcessNetworkData(connectionID, buffer);
                break;
        }
    }

    protected virtual void OnConnection(int connectionID)
    {
        Debug.Log(connectionID);
        bool added = connectionIDs.Add(connectionID);
        if (!added)
        {
            Debug.Log("Duplicate Connection");
        }
    }

    protected virtual void OnDisconnection(int connectionID)
    {
        bool removed = connectionIDs.Remove(connectionID);
        if (!removed)
        {
            Debug.Log("Unknown Connection Lost");
        }
    }

    protected void ProcessNetworkData(int connectionID, byte[] buffer)
    {
        MemoryStream stream = new MemoryStream(buffer);
        BinaryReader reader = new BinaryReader(stream);

        while (stream.Position != buffer.Length)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            Networkidentities[packetType].Notify(new IncomingNetworkStreamReaderMessage(reader, connectionID, packetType));
        }
    }

    void OnApplicationQuit()
    {
        // Gracefully disconnect
        if (hostID != -1 && connectionIDs.Count > 0)
        {
            byte error;

            foreach (int connectionID in connectionIDs)
            {
                NetworkTransport.Disconnect(hostID, connectionID, out error);
            }
        }
    }
}

public class OutgoingNetworkStreamReaderMessage
{
    public readonly BinaryWriter writer; //observers write their set of data (format will vary) in order, starting with a PacketType (the enum) as a byte

    public OutgoingNetworkStreamReaderMessage(BinaryWriter writer)
    {
        this.writer = writer;
    }
}

public class IncomingNetworkStreamReaderMessage
{
    public readonly BinaryReader reader; //observers read their set of data (format will vary), which moves the stream to the next set of data
    public readonly int connectionID;
    public readonly PacketType packetType;

    public IncomingNetworkStreamReaderMessage(BinaryReader reader, int connectionID, PacketType packetType)
    {
        this.reader = reader;
        this.connectionID = connectionID;
        this.packetType = packetType;
    }
}

public interface INetworkable : IObserver<OutgoingNetworkStreamReaderMessage>, IObserver<IncomingNetworkStreamReaderMessage>
{
    PacketType[] packetTypes { get; } //ensure that the programmer has assigned packet types
    //these should be unique per class/format/purpose, for hopefully obvious reasons
}

public static class BinaryReadWriteExtension
{
    public static void Write(this BinaryWriter writer, Vector2 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
    }

    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }
}