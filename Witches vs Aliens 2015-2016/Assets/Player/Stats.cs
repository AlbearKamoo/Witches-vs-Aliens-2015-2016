using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {
    public Side side { get; set; }
    public int playerID { get; set; }
    public NetworkMode networkMode { get; set; }
    
    static int nextID = 0;
    public static int nextPlayerID() //note: may want to add looping because these are sent over the network as bytes [0-255]
    {
        return nextID++;
    }
}