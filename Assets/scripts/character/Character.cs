using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



public class CharacterSyncMsg : MessageBase
{
    public int teamID;
    public int id;
    public float health;
    public Vector3 pos;
}

public class Character : NetworkBehaviour {


    public int teamID = 0;
    public int id = 0;
    public float damage = 1f;
    public float attackDistance = 1f;
    public float viewDistance = 10f;

    public float health = 100f;

    float t_attack = 0;
    Vector3 targetPos = Vector3.zero;


	void Start ()
    {
        CharacterManager.AddCharacter(this);

        var range = transform.FindChild("range");
        if (range != null) range.transform.localScale = new Vector3(attackDistance, 0.2f, attackDistance);
	}
	
	void Update ()
    {
        // update health visuals
        float f = Mathf.Max(0, (health / 100f) * 0.7f);
        transform.localScale = new Vector3(0.3f + f, 0.3f + f, 0.3f + f);

        // move character
        if(targetPos != Vector3.zero)
        {
            // calc direction
            var dir = (targetPos - transform.position).normalized * 0.06f;
            transform.position = transform.position + dir;

            // check if arrived at target
            if(Vector3.Distance(targetPos, transform.position) < 0.3f)
            {
                targetPos = Vector3.zero;
            }
        }

        // check if dead
        if (health <= 0f)
        {
            OnDead();
        }
	}


    public bool CanAttack()
    {
        t_attack += Time.deltaTime;
        if (t_attack > 0.5f)
        {
            t_attack -= 0.5f;
            return true;
        }

        return false;
    }

    public void OnAttack(float dmg)
    {
        health -= dmg;
    }

    public void MoveTo(Vector3 pos)
    {
        targetPos = pos;
    }

    void OnDead()
    {
        CharacterManager.RemoveCharacter(this);
        Destroy(gameObject);
    }

    public CharacterSyncMsg GetStateMsg()
    {
        return new CharacterSyncMsg(){
            teamID = teamID,
            id = id,
            health = health,
            pos = transform.position
        };
    }

    public void UpdateState(CharacterSyncMsg msg)
    {
        health = msg.health;
        transform.position = msg.pos;
    }
}
