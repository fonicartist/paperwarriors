using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitspark : MonoBehaviour
{
    private Animator anim;
    private int sparkNo;

    // Initialization before Start()
    void Awake()
    {
        // Gain access to animator and start firebolt animation
        anim = GetComponentInChildren<Animator>();
    }

    // Used for initialization
    void Start()
    {
        switch (sparkNo)
        {
            case 1: anim.SetTrigger("Hit1");
                break;
            case 2: anim.SetTrigger("Hit2");
                break;
            case 3: anim.SetTrigger("Hit3");
                break;
            case 4: anim.SetTrigger("Hit4");
                break;
            case 5: anim.SetTrigger("Hit5");
                break;
        }
    }

    void Update()
    {
        if (done())
        {
            print("Deleting hitspark");
            Destroy(gameObject);
        }
    }

    // Check if projectile is done with all animations
    bool done()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Default");
    }

    public void setNo(int number)
    {
        sparkNo = number;
    }
}
