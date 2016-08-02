using UnityEngine;
using System.Collections;
using System;

public class PlayerNetComp : MonoBehaviour {

    float t_msg = 0;

	void Start () {
	}
	

	void Update ()
    {
        t_msg += Time.deltaTime;
        if (t_msg > DDNet.updateTick)
        {
            t_msg -= DDNet.updateTick;
            SendUpdate();
        }
	}

    void SendUpdate()
    {
        Byte[] buffer = DDNet.GetSendBuffer();


        DDNet.BufferWriteFloat(buffer, 0, transform.position.x);
        DDNet.BufferWriteFloat(buffer, 4, transform.position.z);


        DDNet.SendBuffer();

    }
}
