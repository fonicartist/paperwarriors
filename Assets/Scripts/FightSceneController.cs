using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneController : MonoBehaviour {

    // Game Text
    public GameObject round1Text,
                      round2Text,
                      round3Text,
                      fightText,
                      player1WinsText,
                      player2WinsText,
                      timesUpText;
    public GameObject stage1, stage2, stage3;
    public GameObject clip1, clip2, clip3, clip4;

    // Game Objects
    private Timer timer;
    private GameObject currentRound;
    private GameObject[] players;
    private GameObject[] stages;

    // Variables
    private int p1WinCount,
                p2WinCount,
                roundNumber,
                p1Char,
                p2Char,
                stageChoice;
    private bool calledCoroutineAlready;

	// Use this for initialization
	void Start () {
        calledCoroutineAlready = false;

        // Get info on the characters chosen during the select screen
        p1Char = PlayerPrefs.GetInt("P1Choice", 1);
        p2Char = PlayerPrefs.GetInt("P2Choice", 0);

        // Assign player objects the correct instantiation
        print("Player 1 chose the character: " + p1Char);
        print("Player 2 chose the character: " + p2Char);

        // Get the current round number and increase it by 1. 
        // If this is the first match, RoundNumber should start at 0
        // Then round 1 will begin.
        roundNumber = PlayerPrefs.GetInt("RoundNumber", 0);
        roundNumber++;
        switch (roundNumber)
        {
            case 1: currentRound = round1Text;
                break;
            case 2: currentRound = round2Text;
                break;
            default: currentRound = round3Text;
                break;
        }

        // Store objects an arrays for reference
        players = GameObject.FindGameObjectsWithTag("Player");
        stages = new GameObject[]{stage1, stage2, stage3};
        print(stages.Length);

        // Disable all stages then enable the one chosen by players.
        foreach (GameObject stage in stages)
            stage.SetActive(false);
        stages[PlayerPrefs.GetInt("StageChoice", 1)].SetActive(true);

        // Determine if player 2 is in the first slot
        if (players[0].name.Equals("Player 2")) {
            GameObject temp = players[0];
            players[0] = players[1];
            players[2] = temp;
        }
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();

        // Load in the current win counts
        p1WinCount = PlayerPrefs.GetInt("Player1Wins", 0);
        p2WinCount = PlayerPrefs.GetInt("Player2Wins", 0);
        print(p1WinCount + " " + p2WinCount);
        displayWins();

        StartCoroutine(matchStart());
        Time.timeScale = 0f;
        //if (!round1Text.activeSelf)
        //    round1Text.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < players.Length; i++)
            if (players[i].GetComponentInChildren<PlayerController>().getHealthPercent() == 0 && !calledCoroutineAlready)
            {
                calledCoroutineAlready = true;
                StartCoroutine(playerWins(i)); 
            }

        if (timer.getTime() == 0 && !calledCoroutineAlready)
        {
            calledCoroutineAlready = true;
            StartCoroutine(playerWinsByTimeout());
        }

	}

    IEnumerator matchStart()
    {
        yield return new WaitForSecondsRealtime(.5f);
        currentRound.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        currentRound.SetActive(false);
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
            // Player 2 Wins
            case 0:
                p2WinCount++;
                PlayerPrefs.SetInt("Player2Wins", p2WinCount);
                player2WinsText.SetActive(true);
                break;
            // Player 1 Wins
            case 1:
                p1WinCount++;
                PlayerPrefs.SetInt("Player1Wins", p1WinCount);
                player1WinsText.SetActive(true);
                break;
        }
        displayWins();
        print("Player has died");
        print(roundNumber);
        PlayerPrefs.SetInt("RoundNumber", roundNumber);
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
        if (players[0].GetComponentInChildren<PlayerController>().getHealthPercent() < players[1].GetComponentInChildren<PlayerController>().getHealthPercent())
            StartCoroutine(playerWins(0));
        else
            StartCoroutine(playerWins(1));

    }

    void displayWins()
    {
        // Show Player 1 Wins
        if (p1WinCount > 0)
        {
            clip1.SetActive(true);
            if (p1WinCount > 1)
                clip2.SetActive(true);
        }
        else
        {
            clip1.SetActive(false);
            clip2.SetActive(false);
        }

        // Show Player 2 Wins
        if (p2WinCount > 0)
        {
            clip3.SetActive(true);
            if (p2WinCount > 1)
                clip4.SetActive(true);
        }
        else
        {
            clip3.SetActive(false);
            clip4.SetActive(false);
        }

    }

    void reload()
    {
        // Delete round statistics when a match is over (has reached 3 rounds or two wins from one person)
        if (roundNumber >= 3 || p1WinCount >= 2 || p2WinCount >= 2)
        {
            PlayerPrefs.DeleteKey("Player1Wins");
            PlayerPrefs.DeleteKey("Player2Wins");
            PlayerPrefs.DeleteKey("RoundNumber");
            print("Loading Title");
            SceneManager.LoadScene("TitleVideo");
            return;
        }
        SceneManager.LoadScene("FightingScene");
    }

}
