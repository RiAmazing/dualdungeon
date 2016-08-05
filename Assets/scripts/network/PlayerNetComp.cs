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
        DDNet.SendJson(DDNet.ToJsonObj(
            DDNet.ToJsonVal("x", transform.position.x),
            DDNet.ToJsonVal("z", transform.position.z)
        ));
    }
}
