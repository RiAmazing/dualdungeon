using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    Character character;

	void Start () 
    {
        character = GetComponent<Character>();
	}
	
	void Update () {
	

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out hit))
            {
                character.MoveTo(hit.point + new Vector3(0f, 0.5f, 0f));
            }
        }
	}
}
