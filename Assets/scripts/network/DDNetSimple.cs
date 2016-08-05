using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;
using UnityEngine.Networking;


public class DDNetSimple : NetworkDiscovery
{

    static DDNetSimple instance = null;

    public int port = 7755;
    public float t_searchTimeout = 5f;

    bool searching = false;
    bool server = false;

    void Awake()
    {
        instance = this;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("recived broadcast");
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }


    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }

    void Update()
    {
        UpdateDiscovery();

        Debug.Log("bcast: " + broadcastsReceived.Count.ToString());
    }

    void UpdateDiscovery()
    {
        t_searchTimeout -= Time.deltaTime;    // update connection timer

        // initially: search for games
        if (t_searchTimeout > 0f && !searching)
        {
            Debug.Log("attempt client");
            Initialize();
            StartAsClient();
            searching = true;
        }
        // after timeout: start server
        if (!server && t_searchTimeout < 0f)
        {
            // stop searching
            searching = false;
            server = true;
            StopBroadcast();
            Initialize();
            StartAsServer();

            StartServer();
        }
    }

    void StartServer()
    {
        Debug.Log("start server");
        NetworkServer.Listen(port);
    }
}
