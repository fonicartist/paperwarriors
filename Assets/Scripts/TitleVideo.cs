using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TitleVideo : MonoBehaviour {

    public GameObject cursor;

    private int index;
    private enum menuIndex : int {Start, Controls, Quit};
    private KeyCode[] confirmKeys = { KeyCode.F, KeyCode.G, KeyCode.H, 
                                      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
                                      KeyCode.Return, KeyCode.Space};
    private Vector3 cursorPos;
    bool musicCheck;
	
    // Use this for initialization
    void Start() {
        index = 0;
        cursorPos = cursor.transform.localPosition;
        musicCheck = false;
    }

	// Update is called once per frame
	void Update () {
        if (!musicCheck)
        {
            //FindObjectOfType<AudioManager>().stop();
            musicCheck = true;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            print("pressing up");
            if (index > 0)
            {
                index--;
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            print("pressing down");
            if (index < 2)
            {
                index++;
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
        }

        placeCursor();
        if (Input.anyKeyDown)
            checkInput();

    }

    void placeCursor()
    {
        cursor.transform.SetPositionAndRotation(new Vector3(cursorPos.x, -1.26f - .8f * index, cursorPos.z), new Quaternion());
    }

    void checkInput()
    {
        foreach (KeyCode key in confirmKeys) {
            if (Input.GetKeyDown(key))
            {
                FindObjectOfType<AudioManager>().play("CursorSelect");
                switch (index)
                {
                    case 0: SceneManager.LoadScene("CharacterSelect"); 
                        break;
                    case 1: SceneManager.LoadScene("ControlsScene");
                        break;
                    case 2: SceneManager.LoadScene("CreditsScene"); 
                        break;
                    default: break;
                }
            }
        }

    }

   
}
