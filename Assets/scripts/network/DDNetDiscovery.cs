using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DDNetDiscovery : NetworkDiscovery {

    public DDNet ddnet;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        ddnet.OnReceivedBroadcast(fromAddress, data);
    }
}
