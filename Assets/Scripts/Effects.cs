using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    private Animator anim;
    private int effectNo;

    // Initialization before Start()
    void Awake()
    {
        // Gain access to animator and start firebolt animation
        anim = GetComponentInChildren<Animator>();
    }

    // Used for initialization
    void Start()
    {
        switch (effectNo)
        {
            case 1: anim.SetTrigger("Jump");
                break;
            case 2: anim.SetTrigger("Land");
                break;
            case 3: anim.SetTrigger("FDash");
                break;
            case 4: anim.SetTrigger("BDash");
                break;
            case 5: anim.SetTrigger("Block");
                break;
        }
    }

    void Update()
    {
        if (done())
        {
            print("Deleting effect");
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
        effectNo = number;
    }
}
