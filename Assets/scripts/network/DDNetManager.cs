using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DDNetManager : NetworkManager {


	public override void OnServerConnect(NetworkConnection conn)
	{
		//Debug.Log ("OnPlayerConnected " + conn.connectionId.ToString());
	}

	public virtual void OnClientConnect(NetworkConnection conn)
	{
		Debug.Log("onclientconnect");
	}

}
