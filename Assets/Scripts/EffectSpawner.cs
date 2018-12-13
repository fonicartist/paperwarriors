using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}

    public void castFirebolt()
    {
        GetComponent<Animator>().SetTrigger("Fire");
        FindObjectOfType<AudioManager>().play("FireThrow");
    }

    public void cancel()
    {
        if (isCasting())
            GetComponent<Animator>().SetTrigger("Blocked");
    }

    bool isCasting()
    {
        return GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Firebolt");
    }
}
