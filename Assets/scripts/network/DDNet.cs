using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Networking;


public class DDNetMsgType 
{

    public static short PING = 1000;

};



public class DDNet : MonoBehaviour
{

    static DDNet instance = null;   // for singleton like usage
    DDNetDiscovery netDiscovery;    // discover games in local network
    DDNetManager netManager;        // needed for access to Network HLAPI

    public int portBroadcast = 7754;
    public int portGame = 7755;
    public float discoveryTimeout = 8f;
    float t_searchTimeout = 8f;

    bool isSearching = false;
    bool isConnected = false;
    bool isServer = false;


    void Awake()
    {
        instance = this;
        t_searchTimeout = discoveryTimeout;

        // init netdiscovery
        netDiscovery = gameObject.AddComponent<DDNetDiscovery>();
        netDiscovery.ddnet = this;
        netDiscovery.broadcastPort = portBroadcast;
        netDiscovery.showGUI = false;
        netDiscovery.useNetworkManager = false;

        // init netmanager
        netManager = gameObject.GetComponent<DDNetManager>();
        netManager.networkPort = portGame;
        //gameObject.AddComponent<NetworkManagerHUD>();
    }

    void Update()
    {
        UpdateDiscovery();

    }


    void UpdateDiscovery()
    {
        if (netDiscovery == null || t_searchTimeout < -1f) return;
        t_searchTimeout -= Time.deltaTime;    // update connection timer

        // initially: search for games
        if (!isSearching && t_searchTimeout > 1f)
        {
            Debug.Log("DDNet: Searching host ...");
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
        }
    }


    // ---------------------------- CLIENT -------------------------------

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        if(netManager.client != null) return;

        // stop broadcasting
        Destroy(netDiscovery);
        netDiscovery = null;
        isSearching = false;

        // parse address
        netManager.networkAddress = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
        Debug.Log("connecting to: " + netManager.networkAddress + ":" + portGame.ToString());
        netManager.StartClient();
        RegisterClientHandlers();
    }

    void RegisterClientHandlers()
    {
        netManager.client.RegisterHandler(MsgType.Connect, OnConnectToServer);
    }

    // client function
    public void OnConnectToServer(NetworkMessage netMsg)
    {
        Debug.Log("Connected!");
        isConnected = true;

        // set connection to ready and spawn player
        ClientScene.Ready(netManager.client.connection);
        ClientScene.AddPlayer((short)netManager.client.connection.connectionId);
    }



    // ---------------------------- SERVER -------------------------------


    void StartServer()
    {
        Debug.Log("DDNet: Hosting at " + Network.player.ipAddress + ":" + portGame);

        // start NetworkDiscovery broadcast
        isServer = true;
        netDiscovery.Initialize();
        netDiscovery.StartAsServer();

        netManager.StartHost(); // start server and client via NetworkManager

        // register server handlers
        // some handlers should be overrides of NetworkManager functions e.g. OnServerConnect ...

        // register client handlers
        RegisterClientHandlers();
    }

    // -------- DEBUG -------------------------------------

    void OnGUI()
    {
        // Display network state
        if(isSearching) GUI.Box(new Rect(10, 10, 220, 20), "searching ...");
        else if(isServer) GUI.Box(new Rect(10, 10, 220, 20), "HOST");
        else if(isConnected) GUI.Box(new Rect(10, 10, 220, 20), "connected!");
        else if(netManager.client != null) GUI.Box(new Rect(10, 10, 220, 20), "connecting to " + netManager.networkAddress + ":" + portGame);


    }

}
