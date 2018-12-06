using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneController : MonoBehaviour {

    public GameObject round1Text,
                      round2Text,
                      round3Text,
                      fightText,
                      player1WinsText,
                      player2WinsText,
                      timesUpText;

    private Timer timer;
    private GameObject[] players;
    private int p1WinCount,
                p2WinCount;

	// Use this for initialization
	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");

        // Determine if player 2 is in the first slot
        if (players[0].name.Equals("Player 2")) {
            GameObject temp = players[0];
            players[0] = players[1];
            players[2] = temp;
        }
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        p1WinCount = 0;
        p2WinCount = 0;
        StartCoroutine(matchStart());
        Time.timeScale = 0f;
        if (!round1Text.activeSelf)
            round1Text.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < players.Length; i++)
            if (players[i].GetComponentInChildren<P1Move>().getHealthPercent() == 0)
                StartCoroutine(playerWins(i));

        if (timer.getTime() == 0)
        {
            StartCoroutine(playerWinsByTimeout());
        }

	}

    IEnumerator matchStart()
    {
        yield return new WaitForSecondsRealtime(.5f);
        round1Text.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        round1Text.SetActive(false);
        yield return new WaitForSecondsRealtime(.25f);
        fightText.SetActive(true);
        yield return new WaitForSecondsRealtime(.8f);
        Time.timeScale = 1f;
        fightText.SetActive(false);
    }

    IEnumerator playerWins(int playerThatLost)
    {
        switch (playerThatLost)
        {
            case 0:
                player2WinsText.SetActive(true);
                break;
            case 1:
                player1WinsText.SetActive(true);
                break;
        }
        print("Player has died");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2);
        reload();
    }

    IEnumerator playerWinsByTimeout()
    {
        timesUpText.SetActive(true);
        print("Player won by timeout");
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2);
        timesUpText.SetActive(false);
        if (players[0].GetComponentInChildren<P1Move>().getHealthPercent() < players[1].GetComponentInChildren<P1Move>().getHealthPercent())
            StartCoroutine(playerWins(0));
        else
            StartCoroutine(playerWins(1));

    }

    void reload()
    {
        
        SceneManager.LoadScene("FightingScene");
    }

}
