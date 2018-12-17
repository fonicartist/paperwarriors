﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalagmite : MonoBehaviour {

    public Rigidbody2D body;
    private Animator anim;

    // Initialization before Start()
    void Awake()
    {
        // Gain access to animator and start firebolt animation
        anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("Rise");
    }

    // Used for initialization
    void Start()
    {
        FindObjectOfType<AudioManager>().play("Quake");
    }

    void Update()
    {
        if (done())
            Destroy(gameObject);
    }

    // Check if projectile is done with all animations
    bool done()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Default");
    }

    public int getType()
    {
        return 2;
    }
}