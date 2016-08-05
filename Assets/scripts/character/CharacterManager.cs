using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

    static CharacterManager instance;

    List<List<Character>> teams = new List<List<Character>>();

	void Awake ()
    {
        instance = this; // singleton usage

        teams.Add(new List<Character>()); // player team
        teams.Add(new List<Character>()); // enemy team
	}
	
	void Update () 
    {
        // for each team iterate all characters and attempt attack
	    for(var t = 0; t < teams.Count; t++)
        {
            for(var c = 0; c < teams[t].Count; c++)
            {
                AttemptAttack(teams[t][c]);
            }
        }
	}

    void AttemptAttack(Character attacker)
    {
        // if attacker can attack
        if (attacker.CanAttack())
        {
            for (var t = 0; t < teams.Count; t++)
            {
                // iterate all teams, except own team
                if (t != attacker.teamID)
                {
                    for (var c = 0; c < teams[t].Count; c++)
                    {
                        if(attacker.attackDistance > Vector3.Distance(attacker.transform.position, teams[t][c].transform.position))
                        {
                            teams[t][c].OnAttack(attacker.damage);
                        }
                    }
                }
            }
        }
    }

    public static void AddCharacter(Character character)
    {
        instance.teams[character.teamID].Add(character);
    }
    public static void RemoveCharacter(Character character)
    {
        instance.teams[character.teamID].Remove(character);
    }

    public static Character GetClosestEnemy(Character character)
    {
        var dist = character.viewDistance;
        Character enemy = null;

        for (var t = 0; t < instance.teams.Count; t++)
        {
            // iterate all teams, except own team
            if (t != character.teamID)
            {
                for (var c = 0; c < instance.teams[t].Count; c++)
                {
                    if(Vector3.Distance(character.transform.position, instance.teams[t][c].transform.position) < dist)
                    {
                        dist = Vector3.Distance(character.transform.position, instance.teams[t][c].transform.position);
                        enemy = instance.teams[t][c];
                    }
                }
            }
        }

        return enemy;
    }
}
