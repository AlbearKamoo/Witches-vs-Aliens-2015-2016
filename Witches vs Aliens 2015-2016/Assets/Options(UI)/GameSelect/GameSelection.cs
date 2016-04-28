using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameSelection : MonoBehaviour {

    [SerializeField]
    protected Toggle warmupEnabled;

    [SerializeField]
    protected Toggle balancedTeamsEnabled;

    [SerializeField]
    protected Toggle eightBitEnabled;

    [SerializeField]
    protected Toggle colorBoundsEnabled;

    [SerializeField]
    protected Dropdown networkingMode;

    [SerializeField]
    protected InputField IP;

    [SerializeField]
    protected Transform localDiscoveryParent;

    [SerializeField]
    protected GameObject serverPrefab;

    [SerializeField]
    protected GameObject clientPrefab;

    [SerializeField]
    protected GameObject localDiscoveryPrefab;

    [SerializeField]
    protected GameObject localDiscoveryUIEntry;

    [SerializeField]
    protected string characterSelectSceneName;

    //static to easily pass it between scenes
    public static bool warmupActive = false; 
    public static bool balancedTeams = true;
    public static bool eightBit = false;
    public static bool colorBounds = true;
    State currentState;

    void Start()
    {
        currentState = new MainState(this);
        currentState.Start();
        (IP.placeholder as Text).text = Format.localIPAddress();
    }

    private interface State
    {
        void Start();
        void StartGame();
    }

    /// <summary>
    /// Called by a UI button.
    /// </summary>
    public void StartGame()
    {
        currentState.StartGame();
    }

    private class MainState : State, IObserver<LocalNetworkDiscoveryMessage>
    {
        private GameSelection host;
        NetworkDiscovery localDiscoveryServer;
        NetworkDiscovery localDiscoveryClient;
        Dictionary<string, GameObject> localAddresses;

        public MainState(GameSelection host)
        {
            this.host = host;
            localAddresses = new Dictionary<string, GameObject>();
        }

        public void Start()
        {
            (host.IP.placeholder as Text).text = Format.localIPAddress();
            localDiscoveryClient = Instantiate(host.localDiscoveryPrefab).GetComponent<NetworkDiscovery>();
            localDiscoveryServer = Instantiate(host.localDiscoveryPrefab).GetComponent<NetworkDiscovery>();

            localDiscoveryClient.Initialize();
            localDiscoveryClient.StartAsClient();
            localDiscoveryClient.Subscribe<LocalNetworkDiscoveryMessage>(this);

            localDiscoveryServer.Initialize();
            localDiscoveryServer.StartAsServer();
        }

        public void Notify(LocalNetworkDiscoveryMessage m)
        {
            if (!localAddresses.ContainsKey(m.fromAddress))
            {
                GameObject instantiatedUIEntry = Instantiate(host.localDiscoveryUIEntry);
                localAddresses[m.fromAddress] = instantiatedUIEntry;
                instantiatedUIEntry.GetComponentInChildren<Text>().text = m.fromAddress;
                instantiatedUIEntry.transform.SetParent(host.localDiscoveryParent, false);
            }
        }

        public void StartGame()
        {
            //remove our local stuff
            Destroy(localDiscoveryClient.gameObject);
            Destroy(localDiscoveryServer.gameObject);

            foreach (GameObject entry in localAddresses.Values)
            {
                Destroy(entry);
            }

            warmupActive = host.warmupEnabled.isOn;
            balancedTeams = host.balancedTeamsEnabled.isOn;
            eightBit = host.eightBitEnabled.isOn;
            colorBounds = host.colorBoundsEnabled.isOn;

            switch (host.networkingMode.value)
            {
                case 0: //Local
                    break;
                case 1: //LAN Server
                    Instantiate(host.serverPrefab);
                    break;
                case 2: //LAN Client
                    GameObject instantiatedClient = Instantiate(host.clientPrefab);
                    instantiatedClient.GetComponent<Client>().serverIP = host.IP.text;
                    break;
            }

            Application.LoadLevel(host.characterSelectSceneName);
        }
    }
}
