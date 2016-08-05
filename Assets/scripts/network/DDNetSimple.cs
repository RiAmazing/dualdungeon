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

    bool discovery = true;
    bool searching = false;
    bool server = false;
    bool client = false;

    void Awake()
    {
        instance = this;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("recived broadcast: " + fromAddress);
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
        discovery = false;
    }

    void Update()
    {
        UpdateDiscovery();

    }

    void UpdateDiscovery()
    {
        if (!discovery) return;
        t_searchTimeout -= Time.deltaTime;    // update connection timer

        // initially: search for games
        if (t_searchTimeout > 1f && !searching)
        {
            Debug.Log("attempt client");
            Initialize();
            StartAsClient(); // network broadcast client
            searching = true;
        }

        // 1sec before timeout: stop searching
        if (t_searchTimeout < 1f && searching)
        {
            searching = false;
            StopBroadcast();
        }

        // after timeout: start server
        if (!server && t_searchTimeout < 0f)
        {
            Debug.Log("start server");
            server = true;
            Initialize();
            StartAsServer(); // network broadcast server
            NetworkServer.Listen(port);
        }
    }
}
