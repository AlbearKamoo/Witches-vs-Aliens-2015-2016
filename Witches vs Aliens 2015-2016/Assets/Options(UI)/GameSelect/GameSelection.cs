using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameSelection : MonoBehaviour {

    [SerializeField]
    protected Toggle warmupEnabled;

    [SerializeField]
    protected Toggle balancedTeamsEnabled;

    [SerializeField]
    protected Dropdown networkingMode;

    [SerializeField]
    protected InputField IP;

    [SerializeField]
    protected GameObject serverPrefab;

    [SerializeField]
    protected GameObject clientPrefab;

    [SerializeField]
    protected string characterSelectSceneName;

    //static to easily pass it between scenes
    public static bool warmupActive; 
    public static bool balancedTeams; 

    void Start()
    {
        (IP.placeholder as Text).text = Format.localIPAddress();
    }

    /// <summary>
    /// Called by a UI button.
    /// </summary>
    public void StartGame()
    {
        warmupActive = warmupEnabled.isOn;
        balancedTeams = balancedTeamsEnabled.isOn;

        switch (networkingMode.value)
        {
            case 0: //Local
                break;
            case 1: //LAN Server
                Instantiate(serverPrefab);
                break;
            case 2: //LAN Client
                GameObject instantiatedClient = Instantiate(clientPrefab);
                instantiatedClient.GetComponent<Client>().serverIP = IP.text;
                break;
        }

        Application.LoadLevel(characterSelectSceneName);
    }
}
