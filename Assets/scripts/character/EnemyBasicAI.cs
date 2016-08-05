using UnityEngine;
using System.Collections;

public class EnemyBasicAI : MonoBehaviour {

    Character character;


	void Start () {
        character = GetComponent<Character>();
	}
	
	void Update ()
    {
        var enemy = CharacterManager.GetClosestEnemy(character);
        if (enemy != null)
        {
            character.MoveTo(enemy.transform.position);
        }
	}
}
