using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour {

    public GameObject fireboltPrefab;
    public GameObject stalagmitePrefab;
    public GameObject hitsparkPrefab;
    public Transform castPoint;
	
	// Update is called once per frame
	void Update () {
        //transform.localScale = ParentPosition.localScale.normalized;
	}

    // Used by Mage character only. Cast Forward Fire Spell
    public void castFirebolt()
    {
        // Firecasting sound
        FindObjectOfType<AudioManager>().play("FireThrow");

        // Set position where the spell casts from
        Vector3 pos = castPoint.position + new Vector3(-1, 0, 0);
        pos.y = -0.65f;

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(1, 0, 0);

        // Assign firebolt to object for further tweaking
        GameObject firebolt = Instantiate(fireboltPrefab, pos, castPoint.rotation);

        // Owner of firebolt is assigned for hit collision purposes
        firebolt.GetComponentInChildren<HitCollider>().owner = GetComponentInParent<PlayerController>();

        // Flips the fireball if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
        {
            firebolt.transform.localScale *= -1;
            firebolt.GetComponent<Firebolt>().direction = 1;
        }
        else 
            firebolt.GetComponent<Firebolt>().direction = 0;
    }

    // Used by Mage character only. Cast Anti-Air Rock Spell
    public void castStalagmite()
    {
        // Earth rumbling sound
        FindObjectOfType<AudioManager>().play("Rumbling");

        // Set position where the spell erupts from
        Vector3 pos = castPoint.position + new Vector3(3, .4f, 0);

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(-3, .4f, 0);
        pos.y = -0.35f;
        GameObject stalagmite = Instantiate(stalagmitePrefab, pos, castPoint.rotation);

        // Owner of firebolt is assigned for hit collision purposes
        stalagmite.GetComponentInChildren<HitCollider>().owner = GetComponentInParent<PlayerController>();

        // Flips the fireball if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            stalagmite.transform.localScale = new Vector2(stalagmite.transform.localScale.x * -1, stalagmite.transform.localScale.y);
    }

    // Used by Mage character only. Cast Aerial Lightning Spell
    public void castLightning()
    {

    }

    // Kick up dust when player jumps
    public void jumpDust()
    {

    }

    // Disturb dust when player lands
    public void landDust()
    {

    }

    // Zoom Zoom
    public void dash()
    {

    }

    // Show hitspark when player lands a hit
    public void hitspark(int number)
    {
        print("Inside hitspark spawner");
        // Get player positions for reference and determine
        // position of hitspark based on player position
        GameObject [] players = GameObject.FindGameObjectsWithTag("Player");
        Vector3 p1Pos = players[0].transform.position;
        Vector3 p2Pos = players[1].transform.position;
        Vector3 center = new Vector2((p1Pos.x + p2Pos.x) / 2, (p1Pos.y + p2Pos.y) / 2);

        // Instantiate hitspark and store a local reference
        GameObject spark = Instantiate(hitsparkPrefab, center, castPoint.rotation);
        spark.GetComponent<Hitspark>().setNo(number);

        // Flips the fireball if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            spark.transform.localScale = new Vector2(spark.transform.localScale.x * -1, spark.transform.localScale.y);
        

    }

}
