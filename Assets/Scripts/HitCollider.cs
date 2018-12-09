﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    public PlayerController owner;

    void OnTriggerEnter2D(Collider2D other) {
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
                        owner.flyback();
                        opponent.flyback();
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
                        opponent.getHurt(owner.getPower());
                        // Play the correct sound for getting hurt depending on the character
                        switch (owner.classType)
                        {
                            case 0: FindObjectOfType<AudioManager>().play("Hit");
                                break;
                            case 1: FindObjectOfType<AudioManager>().play("SwordHit");
                                break;
                            case 2:
                                break;
                        }
                        if (owner.faceRight)
                        {
                            body.velocity = new Vector2(-55, 10);
                            otherBody.velocity = new Vector2(35, 5);
                        }
                        else
                        {
                            body.velocity = new Vector2(55, 10);
                            otherBody.velocity = new Vector2(-35, 5);
                        }
                    }
                }
            }
        }
    }
}
