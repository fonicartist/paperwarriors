using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CharacterSelect : MonoBehaviour
{
    public GameObject p1Cursor, p2Cursor;
    public KeyCode Left1, Right1, Select1, Back1, Left2, Right2, Select2, Back2;

    private enum Character { Swordsman, Fighter, Mage };
    private int p1Choice, p2Choice;
    private bool selected1, selected2;

    private Vector3 p1Pos, p2Pos;

    void Start()
    {
        // Reset fight stats
        PlayerPrefs.DeleteKey("Player1Wins");
        PlayerPrefs.DeleteKey("Player2Wins");
        PlayerPrefs.DeleteKey("RoundNumber");

        p1Choice = (int) Character.Fighter;
        p2Choice = (int) Character.Fighter;
        selected1 = false;
        selected2 = false;
        p1Pos = p1Cursor.transform.position;
        p2Pos = p2Cursor.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for player inputs and do actions accordingly
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(Left1) && !selected1 && p1Choice != (int) Character.Swordsman)
            {
                p1Choice--;
                moveCursor(p1Cursor, -5 + 4 * p1Choice, p1Pos);
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
            else if (Input.GetKeyDown(Right1) && !selected1 && p1Choice != (int)Character.Mage)
            {
                p1Choice++;
                moveCursor(p1Cursor, -5 + 4 * p1Choice, p1Pos);
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
            else if (Input.GetKeyDown(Select1) && !selected1)
            {
                selectCursor(p1Cursor);
                selected1 = true;
                FindObjectOfType<AudioManager>().play("CursorSelect");
            }
            else if (Input.GetKeyDown(Back1) && selected1)
            {
                deselectCursor(p1Cursor);
                selected1 = false;
                FindObjectOfType<AudioManager>().play("CursorBack");
            }
            else if (Input.GetKeyDown(Left2) && !selected2 && p2Choice != (int) Character.Swordsman)
            {
                p2Choice--;
                moveCursor(p2Cursor, -3 + 4 * p2Choice, p2Pos);
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
            else if (Input.GetKeyDown(Right2) && !selected2 && p2Choice != (int) Character.Mage)
            {
                p2Choice++;
                moveCursor(p2Cursor, -3 + 4 * p2Choice, p2Pos);
                FindObjectOfType<AudioManager>().play("CursorMove");
            }
            else if (Input.GetKeyDown(Select2) && !selected2)
            {
                selectCursor(p2Cursor);
                selected2 = true;
                FindObjectOfType<AudioManager>().play("CursorSelect");
            }
            else if (Input.GetKeyDown(Back2) && selected2)
            {
                deselectCursor(p2Cursor);
                selected2 = false;
                FindObjectOfType<AudioManager>().play("CursorBack");
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(Back1) && !selected1) 
                     || (Input.GetKeyDown(Back2) && !selected2))
            { 
                SceneManager.LoadScene("TitleVideo");
                FindObjectOfType<AudioManager>().play("CursorBack");
            }
        }

        // Move onto Stage Select Screen when both players have chosen a character
        if (selected1 && selected2)
        {
            PlayerPrefs.SetInt("P1Choice", p1Choice);
            PlayerPrefs.SetInt("P2Choice", p2Choice);
            SceneManager.LoadScene("StageSelect");
        }

    }

    private void moveCursor(GameObject cursor, int x, Vector3 pos) 
    {
        cursor.transform.SetPositionAndRotation(new Vector3(x, pos.y, pos.z), new Quaternion());
    }

    private void selectCursor(GameObject cursor)
    {
        cursor.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void deselectCursor(GameObject cursor)
    {
        cursor.GetComponent<SpriteRenderer>().enabled = true;
    }

}