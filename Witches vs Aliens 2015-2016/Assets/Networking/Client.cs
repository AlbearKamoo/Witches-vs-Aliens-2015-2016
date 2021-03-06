﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Client : NetworkNode
{
    [SerializeField]
    protected bool simulatedNetworking = false;

    public string serverIP = "127.0.0.1";

    public override NetworkMode networkMode { get { return NetworkMode.LOCALCLIENT; } }

    void OnGUI()
    {
        if (active) // Set when ConnectEvent received
            GUI.Label(new Rect(10, 10, 100, 20), "Connected");
        else
            GUI.Label(new Rect(10, 10, 100, 20), "Disconnected");
    }
    protected override void ConfigureHosts(ConnectionConfig config)
    {
        HostTopology topology = new HostTopology(config, 1);
        if (hostID != -1)
            return;
        #if UNITY_EDITOR
        if(simulatedNetworking)
            hostID = NetworkTransport.AddHostWithSimulator(topology, 200, 400);
        else
            hostID = NetworkTransport.AddHost(topology);
#else
            hostID = NetworkTransport.AddHost(topology);
#endif

        byte error;
        // Cannot tell we're connected until we receive the event at later time
            NetworkTransport.Connect(hostID, serverIP, 25000, 0, out error);
    }
}
