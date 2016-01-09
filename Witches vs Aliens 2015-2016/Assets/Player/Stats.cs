using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
    public Side side { get; set; }
    public int playerID { get; set; }
    public NetworkMode networkState { get; set; }
    
    static int nextID = 0;
    public static int nextPlayerID()
    {
        return nextID++;
    }
}

public enum NetworkMode
{
    UNKNOWN,
    LOCALSERVER, //being controlled locally on the server
    REMOTECLIENT, //controlled by a client, info passed via server
    LOCALCLIENT, //controlled locally, but info passed via server
}