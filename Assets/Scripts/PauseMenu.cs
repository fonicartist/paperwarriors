using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject menu;
    public GameObject cur1, cur2, cur3;

    private enum menuIndex : int {Resume, Restart, MainMenu};
    private KeyCode[] confirmKeys = { KeyCode.F, KeyCode.G, KeyCode.H, 
                                      KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3,
                                      KeyCode.Return, KeyCode.Space};
    private int index;

	// Use this for initialization
	void Start () {
        index = (int)menuIndex.Resume;
        placeCursor();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            print("pressing up");
            if (index > 0)
            {
                index--;
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
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
        switch (index)
        {
            case 0: cur1.SetActive(true);
                cur2.SetActive(false);
                cur3.SetActive(false);
                break;
            case 1: cur2.SetActive(true);
                cur1.SetActive(false);
                cur3.SetActive(false);
                break;
            case 2: cur3.SetActive(true);
                cur1.SetActive(false);
                cur2.SetActive(false);
                break;
        }
    }

    void checkInput()
    {
        foreach (KeyCode key in confirmKeys)
        {
            if (Input.GetKeyDown(key))
            {
                FindObjectOfType<AudioManager>().play("CursorSelect");
                switch (index)
                {
                    case 0: resume();
                        break;
                    case 1: StartCoroutine(load("CharacterSelect"));
                        break;
                    case 2: StartCoroutine(load("TitleVideo"));
                        break;
                    default: break;
                }
            }
        }

    }

    void resume()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    IEnumerator load(string str)
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(str);
    }

}
