﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifebar : MonoBehaviour {

    public PlayerController player;
    private float percentage;
    private Vector3 scale;

	// Use this for initialization
	void Start () {
        percentage = player.getHealthPercent();
        scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        percentage = player.getHealthPercent();
        transform.localScale = new Vector3(scale.x * percentage, scale.y, scale.z);
	}
}
