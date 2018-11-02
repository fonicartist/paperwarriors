using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    P1Move player;

    void OnTriggerEnter2D(Collider2D other)
    {
        player = gameObject.GetComponentInParent<P1Move>();
        if (other.tag == "Ground")
            player.setGrounded();
    }

}
