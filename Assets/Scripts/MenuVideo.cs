using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MenuVideo : MonoBehaviour
{

    public GameObject cursor;
    public GameObject menu;

    private int index;
    private enum menuIndex : int { Start, Controls, Quit };
    private KeyCode[] confirmKeys = { KeyCode.F, KeyCode.G, KeyCode.H,
                                      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
                                      KeyCode.Return, KeyCode.Space};
    private Vector3 cursorPos;

    // Use this for initialization
    void Start()
    {
        index = 0;
        cursorPos = cursor.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            print("pressing up");
            if (index > 0)
                index--;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            print("pressing down");
            if (index < 2)
                index++;
        }

        placeCursor();
        if (Input.anyKeyDown)
            checkInput();

    }

    void placeCursor()
    {
        cursor.transform.SetPositionAndRotation(new Vector3(cursorPos.x, cursorPos.y, cursorPos.z), new Quaternion()); //48.73f - 78f * index
    }

    void checkInput()
    {
        foreach (KeyCode key in confirmKeys)
        {
            if (Input.GetKeyDown(key))
            {
                switch (index)
                {
                    case 0: //Resume
                        menu.GetComponent<SpriteRenderer>().enabled = false;
                        break;
                    case 1: //Restart
                        SceneManager.LoadScene("FightingScene");
                        break;
                    case 2: //Main Menu
                        SceneManager.LoadScene("TitleVideo");
                        break;
                    default: break;
                }
            }
        }

    }
}
