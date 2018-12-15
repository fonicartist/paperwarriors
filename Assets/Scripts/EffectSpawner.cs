using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour {

    public GameObject fireboltPrefab;
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
        // Assign firebolt to object for further tweaking
        GameObject firebolt = Instantiate(fireboltPrefab, castPoint.position, castPoint.rotation);
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

    // Show hitspark when player lands a hit
    public void hitSpark()
    {

    }

}
