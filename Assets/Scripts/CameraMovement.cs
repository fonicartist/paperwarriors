using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public GameObject player1,
                      player2;
    public float xMin, 
                 xMax, 
                 yMin, 
                 yMax;
    private Vector2 center,
                    p1Pos,
                    p2Pos;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        // Get player positions for reference
        p1Pos = player1.transform.localPosition;
        p2Pos = player2.transform.localPosition;

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
