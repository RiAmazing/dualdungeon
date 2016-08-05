using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Networking;


public class DDNetSimple : MonoBehaviour
{

    static DDNetSimple instance = null;
    DDNetDiscovery netDiscovery;

    string serverAddress = null;
    public int portBroadcast = 7754;
    public int portGame = 7755;
    float t_searchTimeout = 8f;

    bool isSearching = false;
    bool isServer = false;
    bool isConnected = false;

    NetworkClient netClient;

    void Awake()
    {
        instance = this;

        // init netdiscovery
        netDiscovery = gameObject.AddComponent<DDNetDiscovery>();
        netDiscovery.ddnet = this;
        netDiscovery.broadcastPort = portBroadcast;
        netDiscovery.showGUI = false;
        netDiscovery.useNetworkManager = false;
    }

    void Update()
    {
        UpdateDiscovery();
    }

    void UpdateDiscovery()
    {
        if (netDiscovery == null) return;
        t_searchTimeout -= Time.deltaTime;    // update connection timer

        // initially: search for games
        if ( !isSearching && t_searchTimeout > 1f)
        {
            Debug.Log("DDNet: Searching server ...");
            netDiscovery.Initialize();
            netDiscovery.StartAsClient(); // network broadcast client
            isSearching = true;
        }

        // 1sec to timeout: stop searching
        if (isSearching && t_searchTimeout < 1f)
        {
            isSearching = false;
            netDiscovery.StopBroadcast();
        }

        // after timeout: start server
        if (!isServer && t_searchTimeout < 0f)
        {
            // start server
            Debug.Log("DDNet: Server started at " + Network.player.ipAddress + ":" + portGame);
            isServer = true;
            netDiscovery.Initialize();
            netDiscovery.StartAsServer(); // network broadcast server
            NetworkServer.Listen(portGame);
        }
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (netClient != null) return;

        // stop broadcasting
        Destroy(netDiscovery);
        netDiscovery = null;
        isSearching = false;

        // parse address
        serverAddress = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
        Debug.Log("connecting to: " + serverAddress + ":" + portGame.ToString());

        // connect to server
        netClient = new NetworkClient();
        netClient.RegisterHandler(MsgType.Connect, OnConnect);
        netClient.Connect(serverAddress, portGame);
    }

    // client function
    public void OnConnect(NetworkMessage netMsg)
    {
        Debug.Log("Connected!");
        isConnected = true;
    }

    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 20), isServer ? "server" : "client");
        GUI.Box(new Rect(10, 40, 100, 20), serverAddress);
        if(!isServer) GUI.Box(new Rect(10, 70, 100, 20), isConnected ? "connected!" : "connecting ...");
    }
}
