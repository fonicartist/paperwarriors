using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    public KeyCode Left1, Right1, Left2, Right2;
    public enum Stage { stage1, stage2, stage3 };

    private Animator anim;
    private Stage _stage;

    void Start()
    {
        anim = GetComponent<Animator>();
        _stage = Stage.stage2;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.anyKeyDown && notTransitioning())
        {
            if (Input.GetKeyDown(Left1) || Input.GetKeyDown(Left2))
            {
                anim.SetTrigger("Pressed Left");
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
            else if (Input.GetKeyDown(Right1) || Input.GetKeyDown(Right2))
            {
                anim.SetTrigger("Pressed Right");
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
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Keypad1) ||
                     Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                PlayerPrefs.SetInt("StageChoice", (int)_stage);
                StartCoroutine(nextScene());
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
                StartCoroutine(prevScene());
        }
           

    }


    // Checks to see if the stages shown on screen are staying still
    bool notTransitioning()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 1") ||
               anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 2") ||
               anim.GetCurrentAnimatorStateInfo(0).IsName("Stage 3");
    }

    IEnumerator nextScene()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneManager.LoadScene("FightingScene");
    }

    IEnumerator prevScene()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneManager.LoadScene("CharacterSelect");
    }

}