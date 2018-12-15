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
                  xMax,
                  previousY,
                  deltaY;
                 
    private Vector2 center,
                    p1Pos,
                    p2Pos;
    private GameObject[] players;
    private Queue<Vector3> posQ;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        // Mobility of the camera is dependent on the stage chosen
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
        posQ = new Queue<Vector3>();
	}
	
	// Update is called once per frame
	void Update () {

        if (players.Length < 2)
            players = GameObject.FindGameObjectsWithTag("Player");

        // Get player positions for reference
        p1Pos = players[0].transform.localPosition;
        p2Pos = players[1].transform.localPosition;

        // Find the center between the players at this frame
        center.x = (p1Pos.x + p2Pos.x) / 2;
        previousY = center.y;
        center.y = 2.5f + (p1Pos.y + p2Pos.y) / 2;

        // Check if the Y is changing a significant amount or not
        deltaY = center.y - previousY;
        if (deltaY < 0)
            deltaY *= -1;
        if (deltaY < .2)
            center.y = previousY;

	}

    // LateUpdate is called at the end of a frame
    void LateUpdate() {

        // Calculate the camera boundaries
        float x = Mathf.Clamp(center.x, xMin, xMax);
        float y = Mathf.Clamp(center.y, yMin, yMax);

        // Set the new camera position if it passes a certain threshold
        posQ.Enqueue(new Vector3(x, y, transform.position.z));

        if (posQ.Count > 6)
            transform.position = posQ.Dequeue();
    }
}
