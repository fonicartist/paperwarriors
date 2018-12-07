using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ControlsScreen : MonoBehaviour
{

    public GameObject keys, howto, howto2, dash;

    private enum Pages { Keys, HowTo, HowTo2, Dash}

    Pages _pages;

    private bool paused;

    void Start()
    {
        _pages = Pages.Keys;
        paused = false;
        keys.SetActive(true);
        howto.SetActive(false);
        howto2.SetActive(false);
        dash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.anyKeyDown && !paused)
        {
            paused = true;
            StartCoroutine(wait());
        }

    }

    IEnumerator wait()
    {
        switch (_pages)
        {
            case Pages.Keys:
                keys.SetActive(false);
                yield return new WaitForSecondsRealtime(.3f);
                howto.SetActive(true);
                break;
            case Pages.HowTo:
                howto.SetActive(false);
                yield return new WaitForSecondsRealtime(.3f);
                howto2.SetActive(true);
                break;
            case Pages.HowTo2:
                howto2.SetActive(false);
                yield return new WaitForSecondsRealtime(.3f);
                dash.SetActive(true);
                break;
            case Pages.Dash:
                dash.SetActive(false);
                yield return new WaitForSecondsRealtime(.3f);
                SceneManager.LoadScene("TitleVideo");
                break;
        }
        _pages++;
        paused = false;
    }
}
