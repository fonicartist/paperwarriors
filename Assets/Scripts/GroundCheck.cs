using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    PlayerController player;

    void OnTriggerEnter2D(Collider2D other)
    {
        player = gameObject.GetComponentInParent<PlayerController>();
        if (other.tag == "Ground")
            player.setGrounded();
    }

}
