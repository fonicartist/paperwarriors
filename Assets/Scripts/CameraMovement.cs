using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // Public
    public float yMin;

    // Private
    private float xVal,
                  yMax,
                  xMin,
                  xMax;
                 
    private Vector2 center,
                    p1Pos,
                    p2Pos;
    private GameObject[] players;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        switch (PlayerPrefs.GetInt("StageChoice", 1))
        {
            case 0: 
                xVal = 2.9f;
                yMax = 2.5f;
                break;
            case 1: 
                xVal = 4.9f;
                yMax = 3.0f;
                break;
            case 2:
                xVal = 3.4f;
                yMax = 4.5f;
                break;
        }
        xMax = xVal;
        xMin = -xVal;
	}
	
	// Update is called once per frame
	void Update () {

        // Get player positions for reference
        p1Pos = players[0].transform.localPosition;
        p2Pos = players[1].transform.localPosition;

        // Find the center between the players
        center.x = (p1Pos.x + p2Pos.x) / 2;
        center.y = 2.5f + (p1Pos.y + p2Pos.y) / 2;

	}

    // LateUpdate is called at the end of a frame
    void LateUpdate() {

        // Calculate the camera boundaries
        float x = Mathf.Clamp(center.x, xMin, xMax);
        float y = Mathf.Clamp(center.y, yMin, yMax);

        // Set the new camera position if it passes a certain threshold

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
