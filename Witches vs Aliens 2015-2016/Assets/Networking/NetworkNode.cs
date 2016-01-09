using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

public abstract class NetworkNode : AbstractNetworkNode, IObservable<OutgoingNetworkStreamReaderMessage>
{
    Observable<OutgoingNetworkStreamReaderMessage> stateSyncObjectsObservable = new Observable<OutgoingNetworkStreamReaderMessage>();
    public Observable<OutgoingNetworkStreamReaderMessage> Observable(IObservable<OutgoingNetworkStreamReaderMessage> self) { return stateSyncObjectsObservable; }

    byte reliableChannel;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SendStateDataCoroutine());
    }

    protected override void ConfigureChannels(ConnectionConfig config)
    {
        reliableChannel = config.AddChannel(QosType.StateUpdate);
    }

    IEnumerator SendStateDataCoroutine()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);

        for (; ; )
        {
            yield return new WaitForSeconds(packetPeriod);

            // Anything to do?
            if (!active)
                continue;

            // Reset stream
            stream.SetLength(0);

            //scripts subscribed to this write their data to the writer
            stateSyncObjectsObservable.Post(new OutgoingNetworkStreamReaderMessage(bw));

            // Send data out
            byte[] buffer = stream.ToArray();
            byte error;
            Debug.Log(string.Format("Sending data size {0}", buffer.Length));
            if (buffer.Length > 0)
            {
                foreach (int connectionID in connectionIDs)
                {
                    NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, buffer.Length, out error);
                }
            }
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
}
