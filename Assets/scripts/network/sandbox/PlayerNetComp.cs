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
        if (t_msg > DDNetOpt.updateTick)
        {
            t_msg -= DDNetOpt.updateTick;
            SendUpdate();
        }
	}

    void SendUpdate()
    {
        DDNetOpt.SendJson(DDNetOpt.ToJsonObj(
            DDNetOpt.ToJsonVal("x", transform.position.x),
            DDNetOpt.ToJsonVal("z", transform.position.z)
        ));
    }
}
