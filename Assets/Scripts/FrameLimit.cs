using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLimit : MonoBehaviour {

    private int TargetFrameRate = 60;

	void Awake () {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrameRate;
	}
	
	void Update () {
		
        if (Application.targetFrameRate != TargetFrameRate)
            Application.targetFrameRate = TargetFrameRate;

    }
}
