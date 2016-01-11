using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Server : NetworkNode
{
    [SerializeField]
    protected bool simulatedNetworking = false;

    protected override void ConfigureHosts(ConnectionConfig config)
    {
        HostTopology topology = new HostTopology(config, 5);

#if UNITY_EDITOR
        // Listen on port 25000
        if(simulatedNetworking)
            hostID = NetworkTransport.AddHost(topology, 25000);
        else
            hostID = NetworkTransport.AddHostWithSimulator(topology, 200, 400, 25000);
#else
        hostID = NetworkTransport.AddHost(topology, 25000);
#endif

        Debug.Log(Format.localIPAddress());
    }
}
