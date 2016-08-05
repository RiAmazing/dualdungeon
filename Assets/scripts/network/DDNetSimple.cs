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

    public int portBroadcast = 7754;
    public int portGame = 7755;
    float t_searchTimeout = 5f;

    bool searching = false;
    bool server = false;
    bool client = false;

    void Awake()
    {
        instance = this;
        netDiscovery = gameObject.AddComponent<DDNetDiscovery>();
        netDiscovery.ddnet = this;
        netDiscovery.broadcastPort = portBroadcast;
        //netDiscovery.showGUI = false;
    }

    void Update()
    {
        UpdateDiscovery();
    }

    void UpdateDiscovery()
    {
        if (t_searchTimeout < -1f) return;
        t_searchTimeout -= Time.deltaTime;    // update connection timer

        // initially: search for games
        if (t_searchTimeout > 1f && !searching)
        {
            Debug.Log("attempt client");
            netDiscovery.Initialize();
            netDiscovery.StartAsClient(); // network broadcast client
            searching = true;
        }

        // after timeout: start server
        if (!server && t_searchTimeout < 0f)
        {
            // stop searching
            searching = false;
            netDiscovery.StopBroadcast();

            // start server
            Debug.Log("start server");
            server = true;
            netDiscovery.Initialize();
            netDiscovery.StartAsServer(); // network broadcast server
            NetworkServer.Listen(portGame);
        }
    }

    public void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("recived broadcast: " + fromAddress);
    }
}
