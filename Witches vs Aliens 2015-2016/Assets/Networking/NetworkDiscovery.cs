using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkDiscovery : MonoBehaviour, IObservable<LocalNetworkDiscoveryMessage>
{
	const int kMaxBroadcastMsgSize = 1024;

	// config data
	[SerializeField]
	public int m_BroadcastPort = 47777;

	[SerializeField]
	public int m_BroadcastKey = 1000;

	[SerializeField]
	public int m_BroadcastVersion = 1;

	[SerializeField]
	public int m_BroadcastSubVersion = 1;

	[SerializeField]
	public string m_BroadcastData = "HELLO";

	// runtime data
	int hostId = -1;
	bool running = false;

	bool m_IsServer = false;
	bool m_IsClient = false;

	byte[] msgOutBuffer = null;
	byte[] msgInBuffer = null;
	HostTopology defaultTopology;

	public bool isServer { get { return m_IsServer; } set { m_IsServer = value; } }
	public bool isClient { get { return m_IsClient; } set { m_IsClient= value; } }

    Observable<LocalNetworkDiscoveryMessage> discoveryObservable = new Observable<LocalNetworkDiscoveryMessage>();
    public Observable<LocalNetworkDiscoveryMessage> Observable(IObservable<LocalNetworkDiscoveryMessage> self) { return discoveryObservable; }

	static byte[] StringToBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	static string BytesToString(byte[] bytes)
	{
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	public bool Initialize()
	{
		if (m_BroadcastData.Length >= kMaxBroadcastMsgSize)
		{
			Debug.LogError("NetworkDiscovery Initialize - data too large. max is " + kMaxBroadcastMsgSize);
			return false;
		}

		if (!NetworkTransport.IsStarted)
		{
			NetworkTransport.Init();
		}

		if (NetworkManager.singleton != null)
		{
			m_BroadcastData = "NetworkManager:"+NetworkManager.singleton.networkAddress + ":" + NetworkManager.singleton.networkPort;
		}

		msgOutBuffer = StringToBytes(m_BroadcastData);
		msgInBuffer = new byte[kMaxBroadcastMsgSize];

		ConnectionConfig cc = new ConnectionConfig();
		cc.AddChannel(QosType.Unreliable);
		defaultTopology = new HostTopology(cc, 1);

		if (m_IsServer)
			StartAsServer();

		if (m_IsClient)
			StartAsClient();

		return true;
	}

	// listen for broadcasts
	public bool StartAsClient()
	{
		if (hostId != -1 || running)
		{
			Debug.LogWarning("NetworkDiscovery StartAsClient already started");
			return false;
		}

		hostId = NetworkTransport.AddHost(defaultTopology, m_BroadcastPort);
		if (hostId == -1)
		{
			Debug.LogError("NetworkDiscovery StartAsClient - addHost failed");
			return false;
		}

		byte error;
		NetworkTransport.SetBroadcastCredentials(hostId, m_BroadcastKey, m_BroadcastVersion, m_BroadcastSubVersion, out error);

		running = true;
		m_IsClient = true;
		Debug.Log("StartAsClient Discovery listening");
		return true;
	}

	// perform actual broadcasts
	public bool StartAsServer()
	{
		if (hostId != -1 || running)
		{
			Debug.LogWarning("NetworkDiscovery StartAsServer already started");
			return false;
		}

		hostId = NetworkTransport.AddHost(defaultTopology, 0);
		if (hostId == -1)
		{
			Debug.LogError("NetworkDiscovery StartAsServer - addHost failed");
			return false;
		}

		byte err;
		if (!NetworkTransport.StartBroadcastDiscovery(hostId, m_BroadcastPort, m_BroadcastKey, m_BroadcastVersion, m_BroadcastSubVersion, msgOutBuffer, msgOutBuffer.Length, 1000, out err))
		{
			Debug.LogError("NetworkDiscovery StartBroadcast failed err: " + err);
			return false;
		}

		running = true;
		m_IsServer = true;
		Debug.Log("StartAsServer Discovery broadcasting");
		DontDestroyOnLoad(gameObject);
		return true;
	}

	public void StopBroadcast()
	{
		if (hostId == -1)
		{
			Debug.LogError("NetworkDiscovery StopBroadcast not initialized");
			return;
		}

		if (!running)
		{
			Debug.LogWarning("NetworkDiscovery StopBroadcast not started");
			return;
		}
		if (m_IsServer)
		{
			NetworkTransport.StopBroadcastDiscovery();
		}

		NetworkTransport.RemoveHost(hostId);
		hostId = -1;
		running = false;
		m_IsServer = false;
		m_IsClient = false;
		msgInBuffer = null;
		Debug.Log("Stopped Discovery broadcasting");
	}

    void OnDestroy()
    {
        if (hostId != -1)
        {
            StopBroadcast();
        }
    }

	void Update()
	{
		if (hostId == -1)
			return;

		if (m_IsServer)
			return;

		int connectionId;
		int channelId;
		int receivedSize;
		byte error;
		NetworkEventType networkEvent = NetworkEventType.DataEvent;

		do
		{
			networkEvent = NetworkTransport.ReceiveFromHost(hostId, out connectionId, out channelId, msgInBuffer, kMaxBroadcastMsgSize, out receivedSize, out error);

			if (networkEvent == NetworkEventType.BroadcastEvent)
			{
				NetworkTransport.GetBroadcastConnectionMessage(hostId, msgInBuffer, kMaxBroadcastMsgSize, out receivedSize, out error);

				string senderAddr;
				int senderPort;
				NetworkTransport.GetBroadcastConnectionInfo(hostId, out senderAddr, out senderPort, out error);

				OnReceivedBroadcast(senderAddr, BytesToString(msgInBuffer));
			}
		} while (networkEvent != NetworkEventType.Nothing);

	}

	public virtual void OnReceivedBroadcast(string fromAddress, string data)
	{
		Debug.Log("Got broadcast from [" + fromAddress + "] " + data);

        discoveryObservable.Post(new LocalNetworkDiscoveryMessage(fromAddress, data));
	}
}

public class LocalNetworkDiscoveryMessage
{
    public readonly string fromAddress;
    public readonly string data;

    public LocalNetworkDiscoveryMessage(string fromAddress, string data)
    {
        this.fromAddress = fromAddress;
        this.data = data;
    }
}
