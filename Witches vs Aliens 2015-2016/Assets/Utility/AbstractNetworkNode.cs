using UnityEngine;
using UnityEngine.Assertions;
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

    public abstract NetworkMode networkMode { get; }

    protected HashSet<int> connectionIDs = new HashSet<int>();
    public HashSet<int> ConnectionIDs { get { return connectionIDs; } }

    protected int hostID = -1;

    protected MemoryStream stream;
    protected BinaryWriter binaryWriter; //shared stream resources to avoid having to construct a new one every time
    public BinaryWriter BinaryWriter { get { return binaryWriter; } } //so other scripts can write

    Dictionary<PacketType, IObserver<IncomingNetworkStreamMessage>> NetworkIdentities = new Dictionary<PacketType, IObserver<IncomingNetworkStreamMessage>>(); //dictates which script handles which messages

    //each int is a different message type. If there are multiple possible destinations (i.e. syncing player positions), the IObserver's Observe method will use statics to determine the proper recipient

    

    protected virtual void Awake()
    {
        stream = new MemoryStream();
        binaryWriter = new BinaryWriter(stream);
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

    public void Subscribe(INetworkable observer, params PacketType[] types)
    {
        for (int i = 0; i < types.Length; i++)
        {
#if UNITY_EDITOR
            if (NetworkIdentities.ContainsKey(types[i]))
            {
                Debug.Log(NetworkIdentities[types[i]]);
                Debug.Log(observer);
            }
#endif
            NetworkIdentities[types[i]] = observer;
        }
    }

    public void Subscribe(INetworkable observer)
    {
        Subscribe(observer, observer.packetTypes);
    }

    public void Unsubscribe(INetworkable observer, params PacketType[] types)
    {
        for (int i = 0; i < types.Length; i++)
            NetworkIdentities.Remove(types[i]);
    }

    public void Unsubscribe(INetworkable observer)
    {
        Unsubscribe(observer, observer.packetTypes);
    }

    public virtual void Clear()
    {
        NetworkIdentities.Clear();
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
                //Debug.Log(string.Format("Got data size {0}", receivedSize));
                Array.Resize(ref buffer, receivedSize);
                ProcessNetworkData(connectionID, buffer);
                break;
        }
    }

    public void Send(IEnumerable<int> connections, byte channelID)
    {
        Assert.IsTrue(hostID != -1);
        // Sends the data in binaryWriter out onto the network
        byte[] buffer = stream.ToArray();
        byte error;
        //Debug.Log(string.Format("Sending data size {0}", buffer.Length));
        Assert.IsTrue(buffer.Length > 0);
        foreach (int connectionID in connections)
        {
            Assert.IsTrue(connectionIDs.Contains(connectionID));
#if UNITY_EDITOR
            if (!NetworkTransport.Send(hostID, connectionID, channelID, buffer, buffer.Length, out error))
            {
                Debug.Log("Networking Error");
                Debug.Log(error);
            }
#else
            NetworkTransport.Send(hostID, connectionID, channelID, buffer, buffer.Length, out error);
#endif
        }

        // Reset stream
        stream.SetLength(0);
    }

    public void Send(byte channelID)
    {
        Send(connectionIDs, channelID);
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
            //Debug.Log(packetType);
            Assert.IsTrue(NetworkIdentities.ContainsKey(packetType));
#if UNITY_EDITOR
            if (!NetworkIdentities.ContainsKey(packetType))
                Debug.Log(packetType);
#endif
            NetworkIdentities[packetType].Notify(new IncomingNetworkStreamMessage(reader, connectionID, packetType));
        }
    }

    void OnDestroy()
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

public class OutgoingNetworkStreamMessage //used for regularly scheduled state syncronization updates
{
    public readonly BinaryWriter writer; //observers write their set of data (format will vary) in order, starting with a PacketType (the enum) as a byte

    public OutgoingNetworkStreamMessage(BinaryWriter writer)
    {
        this.writer = writer;
    }
}

public class IncomingNetworkStreamMessage
{
    public readonly BinaryReader reader; //observers read their set of data (format will vary), which moves the stream to the next set of data
    public readonly int connectionID;
    public readonly PacketType packetType;

    public IncomingNetworkStreamMessage(BinaryReader reader, int connectionID, PacketType packetType)
    {
        this.reader = reader;
        this.connectionID = connectionID;
        this.packetType = packetType;
    }
}

public interface INetworkable : IObserver<IncomingNetworkStreamMessage>
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

    public static void Write(this BinaryWriter writer, PacketType packetType)
    {
        writer.Write((byte)(packetType));
    }

    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }
}

public enum NetworkMode
{
    UNKNOWN, //no networking
    LOCALSERVER, //being controlled locally on the server, broadcasting info to clients
    LOCALCLIENT, //controlled locally on the client, passing input and recieving state from server
    REMOTESERVER, //being run on the server using data from a remote client via networking
    REMOTECLIENT, //being run on the client using data from the server
}

//we shouldn't use open polymorphism for networking stuff, because we would either need to create 3-5 prefabs for each case, or have a spawner script with AddComponent<>() to spawn the correct script during awake
//we can instead have a set of internal classes, one for each case.

public interface IInternalNetworkingPolymorphism
{
    NetworkMode networkMode { get; }
}