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
    public GameObject p1Fighter, p1Swordsman, p1Mage, p2Fighter, p2Swordsman, p2Mage;
    public GameObject clip1, clip2, clip3, clip4;
    public GameObject p1Bar, p2Bar;

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
                stageChoice,
                musicChoice;
    private bool calledCoroutineAlready;

	// Use this for initialization
	void Awake () {
        calledCoroutineAlready = false;

        // Get info on the characters chosen during the select screen
        p1Char = PlayerPrefs.GetInt("P1Choice", 1);
        p2Char = PlayerPrefs.GetInt("P2Choice", 0);

        // Determine which Player 1 character to activate for control based on select screen
        switch (p1Char)
        {
            case 0:
                p1Fighter.SetActive(false);
                p1Mage.SetActive(false);
                p1Swordsman.SetActive(true);
                p1Bar.GetComponent<Lifebar>().setPlayer(p1Swordsman);
                break;
            case 1:
                p1Swordsman.SetActive(false);
                p1Mage.SetActive(false);
                p1Fighter.SetActive(true);
                p1Bar.GetComponent<Lifebar>().setPlayer(p1Fighter);
                break;
            case 2:
                p1Fighter.SetActive(false);
                p1Swordsman.SetActive(false);
                p1Mage.SetActive(true);
                p1Bar.GetComponent<Lifebar>().setPlayer(p1Mage);
                break;
        }

        // Determine which Player 2 character to activate for control based on select screen
        switch (p2Char)
        {
            case 0:
                p2Fighter.SetActive(false);
                p2Mage.SetActive(false);
                p2Swordsman.SetActive(true);
                p2Bar.GetComponent<Lifebar>().setPlayer(p2Swordsman);
                break;
            case 1:
                p2Mage.SetActive(false);
                p2Swordsman.SetActive(false);
                p2Fighter.SetActive(true);
                p2Bar.GetComponent<Lifebar>().setPlayer(p2Fighter);
                break;
            case 2:
                p2Fighter.SetActive(false);
                p2Swordsman.SetActive(false);
                p2Mage.SetActive(true);
                p2Bar.GetComponent<Lifebar>().setPlayer(p2Mage);
                break;
        }

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
        stages = new GameObject[] { stage1, stage2, stage3 };

        // Disable all stages then enable the one chosen by players.
        foreach (GameObject stage in stages)
            stage.SetActive(false);
        stages[PlayerPrefs.GetInt("StageChoice", 1)].SetActive(true);

        // Switch players in array if Player 2 is in the first slot
        if (players[0].GetComponent<PlayerController>().playername.Equals("Player 2")) {
            GameObject temp = players[0];
            players[0] = players[1];
            players[1] = temp;
        }
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();

        // Load in the current win counts
        p1WinCount = PlayerPrefs.GetInt("Player1Wins", 0);
        p2WinCount = PlayerPrefs.GetInt("Player2Wins", 0);
        print(p1WinCount + " " + p2WinCount);
        displayWins();

        // Pauses ingame time and displays round cards
        StartCoroutine(matchStart());
        Time.timeScale = 0f;
	}

    void Start()
    {
        // Determine which music to play based on the stage
        musicChoice = PlayerPrefs.GetInt("MusicChoice", 0);
        if (musicChoice != -1)
        {
            print("Music choice was: " + musicChoice);
            if (musicChoice == 0)
            {
                // Music will play based on the stage selected
                switch (PlayerPrefs.GetInt("StageChoice", 1))
                {
                    case 0: FindObjectOfType<AudioManager>().play("PaperCombat");
                        print("Playing PaperCombat");
                        break;
                    case 1: FindObjectOfType<AudioManager>().play("NihonMori");
                        print("Playing PaperCombat");
                        break;
                    case 2: FindObjectOfType<AudioManager>().play("RocktheDevil");
                        print("Playing PaperCombat");
                        break;
                }
            }
            else
                switch (musicChoice)
                {
                    case 1: FindObjectOfType<AudioManager>().play("PaperCombat");
                        print("Playing PaperCombat");
                        break;
                    case 2: FindObjectOfType<AudioManager>().play("NihonMori");
                        print("Playing PaperCombat");
                        break;
                    case 3: FindObjectOfType<AudioManager>().play("RocktheDevil");
                        print("Playing PaperCombat");
                        break;
                }
        }
        // This lets the Fight Scene Controller know that music is already being played
        PlayerPrefs.SetInt("MusicChoice", -1);
    }
	
	// Update is called once per frame
	void Update () {

        // Check player healths to see if someone is dead
        for (int i = 0; i < players.Length; i++)
            if (players[i].GetComponentInChildren<PlayerController>().getHealthPercent() == 0 && !calledCoroutineAlready)
            {
                calledCoroutineAlready = true;
                StartCoroutine(playerWins(i)); 
            }

        // Stops the match in the event of a timeout
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

    // This function displays the rounds a player has one as paperclips near the health bar
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
            Destroy(FindObjectOfType<AudioManager>().gameObject);
            SceneManager.LoadScene("TitleVideo");
            return;
        }
        SceneManager.LoadScene("FightingScene");
    }

}
