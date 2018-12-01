using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneController : MonoBehaviour {

    public GameObject roundText, 
                      fightText;

    private Timer timer;
    private GameObject[] players;
    private int p1WinCount,
                p2WinCount;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        p1WinCount = 0;
        p2WinCount = 0;
        StartCoroutine(matchStart());
        Time.timeScale = 0f;
        if (!roundText.activeSelf)
            roundText.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

        foreach (GameObject p in players)
            if (p.GetComponentInChildren<P1Move>().getHealthPercent() == 0)
                StartCoroutine(playerWins());

        if (timer.getTime() == 0)
        {
            StartCoroutine(playerWinsByTimeout());
        }

	}

    IEnumerator matchStart()
    {
        yield return new WaitForSecondsRealtime(.5f);
        roundText.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        roundText.SetActive(false);
        yield return new WaitForSecondsRealtime(.25f);
        fightText.SetActive(true);
        yield return new WaitForSecondsRealtime(.8f);
        Time.timeScale = 1f;
        fightText.SetActive(false);
    }

    IEnumerator playerWins()
    {
        print("Player has died");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1);
        reload();
    }

    IEnumerator playerWinsByTimeout()
    {
        print("Player won by timeout");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2);
        reload();
    }

    void reload()
    {
        
        SceneManager.LoadScene("FightingScene");
    }

}
