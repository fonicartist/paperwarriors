using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour {

    public GameObject fireboltPrefab;
    public GameObject stalagmitePrefab;
    public GameObject lightningPrefab;
    public GameObject hitsparkPrefab;
    public GameObject effectsPrefab;
    public Transform castPoint;

    private Vector3 pos;
	
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
        pos = castPoint.position + new Vector3(-1, 0, 0);
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
        pos = castPoint.position + new Vector3(3, .4f, 0);

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(-3f, .4f, 0);
        pos.y = -0.35f;
        GameObject stalagmite = Instantiate(stalagmitePrefab, pos, castPoint.rotation);

        // Owner of stalagmite is assigned for hit collision purposes
        stalagmite.GetComponentInChildren<HitCollider>().owner = GetComponentInParent<PlayerController>();

        // Flips the stalagmite if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            stalagmite.transform.localScale = new Vector2(stalagmite.transform.localScale.x * -1, stalagmite.transform.localScale.y);
    }

    // Used by Mage character only. Cast Aerial Lightning Spell
    public void castLightning()
    {
        // Set position where the spell erupts from
        pos = castPoint.position + new Vector3(4.5f, 0, 0);

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(-4.5f, 0, 0);
        pos.y = 0f;
        GameObject lightning = Instantiate(lightningPrefab, pos, castPoint.rotation);

        // Owner of lightning is assigned for hit collision purposes
        lightning.GetComponentInChildren<HitCollider>().owner = GetComponentInParent<PlayerController>();

        // Flips the lightning if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            lightning.transform.localScale = new Vector2(lightning.transform.localScale.x * -1, lightning.transform.localScale.y);
    }

    // Kick up dust when player jumps
    public void jumpDust()
    {
        print("Inside jump dust spawner");
        // Position of the dust could based on character position
        pos = castPoint.position;
        pos.y = -2.8f;

        // Instantiate jump dust and store a local reference
        GameObject dust = Instantiate(effectsPrefab, pos, castPoint.rotation);
        dust.GetComponent<Effects>().setNo(1);

        // Flips the dust if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            dust.transform.localScale = new Vector2(dust.transform.localScale.x * -1, dust.transform.localScale.y);
    }

    // Disturb dust when player lands
    public void landDust()
    {
        print("Inside land dust spawner");
        // Position of the dust could based on character position
        pos = castPoint.position + new Vector3(.8f, 0, 0);

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(-.8f, 0, 0);
        pos.y = -3.8f;

        // Instantiate jump dust and store a local reference
        GameObject dust = Instantiate(effectsPrefab, pos, castPoint.rotation);
        dust.GetComponent<Effects>().setNo(2);

        // Flips the dust if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            dust.transform.localScale = new Vector2(dust.transform.localScale.x * -1, dust.transform.localScale.y);
    }

    // Zoom Zoom
    public void dash(int number)
    {
        print("Inside dash effect spawner");

        // Player is forwarddashing
        if (number == 0)
        {
            // Position of the effect could based on character position
            pos = castPoint.position + new Vector3(0, -1, 0);

            // Flip the position if player is facing the other way
            if (!GetComponentInParent<PlayerController>().faceRight)
                pos = castPoint.position + new Vector3(0, -1, 0);
        }
        // Player is backdashing
        else
        {
            // Position of the effect could based on character position
            pos = castPoint.position + new Vector3(2f, -1, 0);

            // Flip the position if player is facing the other way
            if (!GetComponentInParent<PlayerController>().faceRight)
                pos = castPoint.position + new Vector3(-2f, -1, 0);
        }
        // Instantiate the effect and store a local reference
        GameObject effect = Instantiate(effectsPrefab, pos, castPoint.rotation);

        // Pass a different value for the animation switch for forwarddash and backdash
        effect.GetComponent<Effects>().setNo(number + 3);

        // Flips the effect if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            effect.transform.localScale = new Vector2(effect.transform.localScale.x * -1, effect.transform.localScale.y);
    }

    // Block effect
    public void block()
    {
        print("Inside block effect spawner");
        // Position of the effect could based on character position
        pos = castPoint.position + new Vector3(1f, 0, 0);

        // Flip the position if player is facing the other way
        if (!GetComponentInParent<PlayerController>().faceRight)
            pos = castPoint.position + new Vector3(-1f, 0, 0);

        // Instantiate the effect and store a local reference
        GameObject block = Instantiate(effectsPrefab, pos, castPoint.rotation);
        block.GetComponent<Effects>().setNo(5);

        // Flips the effect if the player is flipped (originally faces to the right)
        if (!GetComponentInParent<PlayerController>().faceRight)
            block.transform.localScale = new Vector2(block.transform.localScale.x * -1, block.transform.localScale.y);
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
