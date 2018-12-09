using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    public PlayerController player;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Ground")
        {
            player.setGrounded();
        }
    }

}
