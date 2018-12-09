using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    public KeyCode Left1, Right1, Left2, Right2;
    public enum Stage { stage1, stage2, stage3 };
    public enum Music { original, combat, forest, rock };
    public Animator grayAnim;
    public GameObject musicPicker;
    public GameObject[] musicText;

    private Animator anim;
    private Stage _stage;
    private Music _music;
    private bool stageChosen;
    private SpriteRenderer[] grays;

    void Start()
    {
        anim = GetComponent<Animator>();
        Time.timeScale = 1f;
        _stage = Stage.stage2;
        _music = Music.original;
        stageChosen = false;
        grays = grayAnim.GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stageChosen)
        {
            foreach (SpriteRenderer s in grays)
                s.enabled = true;
            musicPicker.SetActive(true);
            foreach (GameObject g in musicText)
                g.SetActive(false);
            musicText[(int) _music].SetActive(true);
        }
        else
        {
            foreach (SpriteRenderer s in grays)
                s.enabled = false;
            musicPicker.SetActive(false);
        }

        if (Input.anyKeyDown && notTransitioning())
        {
            if (Input.GetKeyDown(Left1) || Input.GetKeyDown(Left2))
            {
                if (!stageChosen)
                {
                    anim.SetTrigger("Pressed Left");
                    grayAnim.SetTrigger("Pressed Left");
                    FindObjectOfType<AudioManager>().play("TurnPage");
                    switch ((int)_stage)
                    {
                        case 0: _stage = Stage.stage3;
                            break;
                        case 1: _stage = Stage.stage1;
                            break;
                        case 2: _stage = Stage.stage2;
                            break;
                    }
                }
                else
                {
                    FindObjectOfType<AudioManager>().play("CursorMove");
                    switch ((int)_music)
                    {
                        case 0: _music = Music.rock;
                            break;
                        case 1: _music = Music.original;
                            break;
                        case 2: _music = Music.combat;
                            break;
                        case 3: _music = Music.forest;
                            break;
                    }
                }
            }
            else if (Input.GetKeyDown(Right1) || Input.GetKeyDown(Right2))
            {
                if (!stageChosen)
                {
                    anim.SetTrigger("Pressed Right");
                    grayAnim.SetTrigger("Pressed Right");
                    FindObjectOfType<AudioManager>().play("TurnPage");
                    switch ((int)_stage)
                    {
                        case 0: _stage = Stage.stage2;
                            break;
                        case 1: _stage = Stage.stage3;
                            break;
                        case 2: _stage = Stage.stage1;
                            break;
                    }
                }
                else
                {
                    FindObjectOfType<AudioManager>().play("CursorMove");
                    switch ((int)_music)
                    {
                        case 0: _music = Music.combat;
                            break;
                        case 1: _music = Music.forest;
                            break;
                        case 2: _music = Music.rock;
                            break;
                        case 3: _music = Music.original;
                            break;
                    }
                }
            }
            else if (confirm())
            {
                FindObjectOfType<AudioManager>().play("CursorSelect");
                if (!stageChosen)
                {
                    PlayerPrefs.SetInt("StageChoice", (int)_stage);
                    stageChosen = true;
                }
                else
                {
                    PlayerPrefs.SetInt("MusicChoice", (int)_music);
                    StartCoroutine(nextScene());
                }
            }
            else if (cancel())
            {
                FindObjectOfType<AudioManager>().play("CursorBack");
                if (!stageChosen)
                    StartCoroutine(prevScene());
                else
                    stageChosen = false;
            }
        }
           

    }


    // Checks to see if the stages shown on screen are staying still
    bool notTransitioning()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 1") ||
               anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 2") ||
               anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 3");
    }

    bool confirm()
    {
        return (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.H) ||
                Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Keypad3) ||
                Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space));
    }

    bool cancel()
    {
        return (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Keypad2));
    }

    IEnumerator nextScene()
    {
        yield return new WaitForSecondsRealtime(.4f);
        Destroy(FindObjectOfType<AudioManager>().gameObject);
        yield return new WaitForSecondsRealtime(.2f);
        SceneManager.LoadScene("FightingScene");
    }

    IEnumerator prevScene()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneManager.LoadScene("CharacterSelect");
    }

}