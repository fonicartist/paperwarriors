using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : MonoBehaviour {

    public Rigidbody2D body;
    public int direction;
    private Animator anim;

    // Initialization before Start()
    void Awake()
    {
        // Gain access to animator and start firebolt animation
        anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("Fire");
    }

    // Used for initialization
    void Start()
    {
        // Determine the direction of the firebolt
        switch (direction) { 
            case 0: body.velocity = transform.right * 20;
                break;
            case 1: body.velocity = transform.right * -20;
                break;
        }
    
    }

    void Update()
    {
        // Check if animation state changes
        if (exploded())
            body.velocity = body.velocity * .1f;
        else if (done())
            Destroy(gameObject);
    }

    // Check if currently playing the exploded animation
    bool exploded()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Impact") ||
               anim.GetCurrentAnimatorStateInfo(0).IsName("Impact2");
    }

    // Check if projectile is done with all animations
    bool done()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Default");
    }

}
