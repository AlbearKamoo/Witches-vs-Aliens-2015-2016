using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

//contains game-specific networking stuff

public abstract class NetworkNode : AbstractNetworkNode, IObservable<OutgoingNetworkStreamMessage>
{
    public static NetworkNode self; //there should only be one
    public static NetworkNode node { get { return self; } }

    Observable<OutgoingNetworkStreamMessage> stateSyncObjectsObservable = new Observable<OutgoingNetworkStreamMessage>();
    public Observable<OutgoingNetworkStreamMessage> Observable(IObservable<OutgoingNetworkStreamMessage> self) { return stateSyncObjectsObservable; }

    byte reliableChannel;
    public byte ReliableChannel { get { return reliableChannel; } }
    byte allCostChannel;
    public byte AllCostChannel { get { return allCostChannel; } }

    protected override void Awake()
    {
        base.Awake();
        if (self != null && self != this)
        {
            Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            self = this;
        }
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SendStateDataCoroutine());
    }

    public override void Clear()
    {
        base.Clear();
        stateSyncObjectsObservable.Clear();
    }

    protected override void ConfigureChannels(ConnectionConfig config)
    {
        reliableChannel = config.AddChannel(QosType.StateUpdate);
        allCostChannel = config.AddChannel(QosType.Reliable);
    }

    IEnumerator SendStateDataCoroutine()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(packetPeriod);

            // Anything to do?
            if (!active)
                continue;

            //scripts subscribed to this write their data to the writer
            stateSyncObjectsObservable.Post(new OutgoingNetworkStreamMessage(binaryWriter));

            if(stream.Length != 0)
                Send(connectionIDs, reliableChannel); // Send data out
        }
    }

    /*
    public void Subscribe(INetworkable observer) //subscribes for self-to-self communication. //do NOT use if you are sending messages to be recieved by a different script
    {
        if (this is Server)
        {
            this.Subscribe<OutgoingNetworkStreamReaderMessage>(observer);
        }
        else if (this is Client)
        {
            this.Subscribe(observer, observer.packetTypes);
        }
    }
     * */
}

public enum PacketType
{
    UNKNOWN,
    PUCKLOCATION,
    PLAYERLOCATION,
    PLAYERINPUT,
    PLAYERMOVEMENTABILITY,
    PLAYERGENERICABILITY,
    PLAYERSUPERABILITY,
    SUPERGOALSPAWNING,
    SUPERGOALSCORED,
    REGISTRATIONREQUEST,
    PLAYERREGISTER,
    PLAYERDEREGISTER,
}
