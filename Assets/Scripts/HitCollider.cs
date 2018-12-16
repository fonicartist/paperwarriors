using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    public PlayerController owner;

    void OnTriggerEnter2D(Collider2D other) {
        // Make projectile destroy itself it hits another projectile that's the same type
        if (other.tag == "Effect")
        {
            PlayerController opponent = other.GetComponentInParent<HitCollider>().owner;
            Rigidbody2D body = owner.GetComponentInParent<Rigidbody2D>();
            Rigidbody2D otherBody = opponent.GetComponent<Rigidbody2D>();
            AnimatorStateInfo anim = other.GetComponentInParent<Animator>().GetCurrentAnimatorStateInfo(0);
            
            if (opponent.getAttack())
            {
                int ownerType = owner.getAttackType();
                int otherType = opponent.getAttackType();

                if (ownerType == otherType && owner.classType == 2 && opponent.classType == 2)
                {
                    GetComponent<Animator>().SetTrigger("Clashed");

                    // Play the correct sound based on which spell was blocked
                    switch (owner.getAttackType()) { 
                        case 0: FindObjectOfType<AudioManager>().play("FireHit");
                            break;
                        case 1:
                            break;
                        case 2: FindObjectOfType<AudioManager>().play("RockHit");
                            break;
                    }
                    return;
                }
            }
        }

        // Check for conditions on the other player to determine if damage will be done
        if (other.tag == "PlayerBody") {
            PlayerController opponent = other.gameObject.GetComponentInParent<PlayerController>();
            Rigidbody2D body = owner.GetComponentInParent<Rigidbody2D>();
            Rigidbody2D otherBody = other.GetComponentInParent<Rigidbody2D>();
            AnimatorStateInfo anim = other.GetComponentInParent<Animator>().GetCurrentAnimatorStateInfo(0);
            bool isHurt = false;

            if (anim.IsName("Hurt_Anim") || anim.IsName("Hurt_Air_Anim"))
                isHurt = true;

            if (opponent != null && opponent != owner) {
                bool canHurtOpponent = true;

                // Clashes if the enemy is attacking too. On the same attack, both players are sent to opposite ends of the screen
                // Lunge(0) beats anti-air(2) which beats aerial(1) which beats lunge
                if (opponent.getAttack())
                {
                    int ownerType = owner.getAttackType();
                    int otherType = opponent.getAttackType();
                    canHurtOpponent = false;

                    // Same attack sends players backwards
                    if (ownerType == otherType)
                    {
                        // Owner of collider is not a mage
                        if (owner.classType != 2)
                            owner.flyback();
                        // Owner of the collider is a mage
                        if (owner.classType == 2)
                        {
                            GetComponent<Animator>().SetTrigger("Blocked");
                            FindObjectOfType<AudioManager>().play("FireHit");
                        }
                        // Opponent is not a mage
                        if (opponent.classType != 2)
                            opponent.flyback();
                        // Neither players are a mage
                        if (opponent.classType != 2 && owner.classType != 2)
                            FindObjectOfType<AudioManager>().play("Block");
                        return;
                    }

                    switch (otherType)
                    {
                        // Opponent is using lunge attack
                        case 0:
                            if (ownerType == 1)
                                canHurtOpponent = true;
                            break;
                        // Opponent is using aerial attack
                        case 1:
                            if (ownerType == 2)
                                canHurtOpponent = true;
                            break;
                        // Opponent is using anti-air attack
                        case 2:
                            if (ownerType == 0)
                                canHurtOpponent = true;
                            break;

                        default: break;
                    }
                }
                // Able to hurt the enemy if they are not currently in hit stun
                if (!opponent.isInvincible() && canHurtOpponent && !isHurt)
                {
                    if (opponent.isBlocking())
                    {
                        opponent.block();
                        FindObjectOfType<AudioManager>().play("Block");
                        if (owner.faceRight)
                        {
                            body.velocity = new Vector2(-15, 5);
                            otherBody.velocity = new Vector2(10, 10);
                        }
                        else
                        {
                            body.velocity = new Vector2(15, 10);
                            otherBody.velocity = new Vector2(-10, 10);
                        }
                    }
                    else
                    {
                        // Play the correct sound for getting hurt depending on the character
                        switch (owner.classType)
                        {
                            case 0: FindObjectOfType<AudioManager>().play("Hit");
                                break;
                            case 1: FindObjectOfType<AudioManager>().play("SwordHit");
                                break;
                            case 2:
                                switch(owner.getAttackType())
                                {
                                    case 0: FindObjectOfType<AudioManager>().play("FireHit");
                                        GetComponent<Animator>().SetTrigger("Blocked");
                                        print("Flamed on");
                                        break;
                                    case 1: 
                                        break;
                                    case 2: FindObjectOfType<AudioManager>().play("RockHit");
                                        print("Pounded by rock"); 
                                        break;
                                }
                                break;
                        }

                        // Subtract health
                        opponent.getHurt(owner.getPower());

                        if (owner.faceRight)
                        {
                            // Player doesn't move if casting spell
                            if (owner.classType != 2)
                                body.velocity = new Vector2(-55, 10);

                            // Check if doing uppercut
                            if (owner.getAttackType() == 2)
                                otherBody.velocity = new Vector2(25, 20);
                            else
                                otherBody.velocity = new Vector2(35, 5);
                        }
                        else
                        {
                            // Player doesn't move if casting spell
                            if (owner.classType != 2)
                                body.velocity = new Vector2(55, 10);

                            // Check if doing uppercut
                            if (owner.getAttackType() == 2)
                                otherBody.velocity = new Vector2(-25, 20);
                            else
                                otherBody.velocity = new Vector2(-35, 5);
                        }
                    }
                }
            }
        }
    }
}
