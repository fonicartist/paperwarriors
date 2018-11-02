using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    public GameObject Tens, 
                      Ones;
    public int ROUND_TIME;
    private Animator tenAnim,
                     oneAnim;
    private int roundTime,
                currentTen, 
                currentOne;
    private float deltaTime, 
                  currentTime;

	// Use this for initialization
	void Start () {
        tenAnim = Tens.GetComponent<Animator>();
        oneAnim = Ones.GetComponent<Animator>();
        deltaTime = Time.time;
        roundTime = ROUND_TIME;
	}
	
	// Update is called once per frame
	void Update () {
        currentTime = Time.time;

        if (currentTime - deltaTime > 1) {
            deltaTime = currentTime;
            roundTime--;
        }

        if (roundTime < 0)
            roundTime = 0;

        currentTen = roundTime / 10;
        currentOne = roundTime % 10;

        tenAnim.SetInteger("Time", currentTen);
        oneAnim.SetInteger("Time", currentOne);        

	}

    public int getTime()
    {
        return roundTime;
    }

}
