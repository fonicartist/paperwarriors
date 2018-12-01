using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CharacterSelect : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

        if (Input.anyKeyDown)
            SceneManager.LoadScene("StageSelect");

    }
}