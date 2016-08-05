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
        if (!isSearching && t_searchTimeout > 1f)
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
            StartServer();
            StartClient(true);
        }
    }


    // ---------------------------- CLIENT -------------------------------

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        // stop broadcasting
        Destroy(netDiscovery);
        netDiscovery = null;
        isSearching = false;

        // parse address
        serverAddress = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
        StartClient(false);
    }


    void StartClient(bool local)
    {
        if (netClient != null) return;

        if(local) // client also functions as host
        {
            Debug.Log("connecting to: localhost:" + portGame.ToString());
            netClient = ClientScene.ConnectLocalServer();
        } 
        else // somebody else is hosting
        {
            Debug.Log("connecting to: " + serverAddress + ":" + portGame.ToString());
            netClient = new NetworkClient();
            netClient.Connect(serverAddress, portGame);
        } 

        netClient.RegisterHandler(MsgType.Connect, OnConnectToServer);
    }

    // client function
    public void OnConnectToServer(NetworkMessage netMsg)
    {
        Debug.Log("Connected!");
        isConnected = true;
    }


    // ---------------------------- SERVER -------------------------------


    void StartServer()
    {
        Debug.Log("DDNet: Server started at " + Network.player.ipAddress + ":" + portGame);
        isServer = true;
        netDiscovery.Initialize();
        netDiscovery.StartAsServer(); // network broadcast server
        NetworkServer.Listen(portGame);
        NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnect);
    }


    void OnPlayerConnect(NetworkMessage netMsg)
    {
        Debug.Log("Player Joined!");
    }


    // -------- DEBUG -------------------------------------

    void OnGUI()
    {
        // Display network state
        if(isSearching) GUI.Box(new Rect(10, 10, 220, 20), "searching ...");
        else if(isServer) GUI.Box(new Rect(10, 10, 220, 20), "HOST");
        else if(isConnected) GUI.Box(new Rect(10, 10, 220, 20), "connected!");
        else if(netClient != null) GUI.Box(new Rect(10, 10, 220, 20), "connecting to " + serverAddress + ":" + portGame);
    }
}
