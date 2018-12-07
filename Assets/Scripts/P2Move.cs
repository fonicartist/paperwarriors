using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Move : MonoBehaviour
{
    // CONSTANTS
    private static float GRAVITY = 9.5f;

    // PUBLIC 
    public GameObject otherPlayer;
    public float moveSpeed = 10;
    public float MAX_HEALTH;

    public KeyCode Up, Down, Left, Right, Punch, UpperCut, Aerial;

    // PRIVATE
    private float deltaTime;
    public float health;
    private float[] doubleTapTime;
    private bool faceRight = false,
                 isJumping = false,
                 airDashing = false,
                 canAirDash = false,
                 tappedLeft = false,
                 tappedRight = false,
                 isAttacking;
    private enum AnimNumber : int { Idle, Jumping, ForwardDash, BackDash, Punch, Uppercut, Aerial };
    private int _animNumber;
    private Vector3 pos,
                    otherPos;
    private Vector2 speed,
                    scale;
    private Rigidbody2D body;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        speed = new Vector2(0, 0);
        scale = gameObject.transform.localScale;
        doubleTapTime = new float[2];
        body = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        _animNumber = (int)AnimNumber.Idle;
        health = MAX_HEALTH;
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Global.isPaused)
        {
            // Grab references for position and Time for this frame
            pos = transform.position;
            otherPos = otherPlayer.transform.localPosition;
            deltaTime = Time.deltaTime;

            // Call the function that controls player movement
            movePlayer();

            // Set the animation to be played
            anim.SetInteger("AnimNumber", _animNumber);

            // Ground Attacks
            attack();

            // Aerial Attacks
            airAttack();
        }
    }

    // Moves the player left and right
    void movePlayer()
    {

        // These actions can only be done when not jumping
        if (!isJumping)
        {

            // Determine Left Right Movement
            if (Input.GetKey(Left))
                speed.x = 1;
            else if (Input.GetKey(Right))
                speed.x = -1;
            else
                speed.x = 0;

            // Flip player around if needed
            if (faceRight && pos.x > otherPos.x + 0.5f)
                flipPlayer();
            else if (!faceRight && pos.x < otherPos.x - 0.5f)
                flipPlayer();

            // Call function for handling player jumps
            jump();
        }

        // Updates jumping motion if while isJumping is true
        updateJump();

        // Carry out motion
        transform.Translate(speed.x * moveSpeed * deltaTime, speed.y * moveSpeed * deltaTime, 0);
    }

    // Controls the jumping motion
    void jump()
    {

        // Check for jump command
        if (Input.GetKeyDown(Up))
        {
            speed = new Vector2(0, 4);
            isJumping = true;
            canAirDash = true;
            _animNumber = (int)AnimNumber.Jumping;

            if (Input.GetKey(Left))
            {
                speed.x = -.85f;
            }
            else if (Input.GetKey(Right))
            {
                speed.x = .85f;
            }
        }

    }

    // Update position when jumping
    void updateJump()
    {

        // Check if an air dash is being performed
        if (speed.y > 0 || pos.y > 1.15)
            airDash();

        // Change animation back to jumping if airdash is finished.
        //if (isJumping && Mathf.Abs(body.velocity.x) < moveSpeed / 2)
        //_animNumber = (int)AnimNumber.Jumping;

        // Apply gravity
        if ((speed.y > -4.5f && isJumping) || (!isJumping && pos.y > -1.56))
            speed.y -= GRAVITY * deltaTime;

        // Prevent jumping over/on the other player if too close while in the air
        if (pos.y < 2.3 && speed.y < 0 && !airDashing)
        {
            if (Mathf.Abs(otherPos.x - pos.x) < 0.5f)
            {
                if (pos.x < otherPos.x + 1)
                    transform.SetPositionAndRotation(new Vector3(otherPos.x - 1f, pos.y, pos.z), new Quaternion());
                else if (pos.x > otherPos.x - 1)
                    transform.SetPositionAndRotation(new Vector3(otherPos.x + 1f, pos.y, pos.z), new Quaternion());
                speed.x = 0;
            }
            else if (Mathf.Abs(otherPos.x - pos.x) < 1.5f)
            {
                if (speed.x > .8f)
                    speed.x -= .5f;
                else if (speed.x < -.8f)
                    speed.x += .5f;
                else
                    speed.x = 0;
            }
        }
        else if (pos.y < 1.2)
            airDashing = false;

        // Set vertical speed to zero when landing at ground level
        if (pos.y <= -1.59f && speed.y < 0)
        {
            speed.y = 0;
            isJumping = false;
            canAirDash = false;
            tappedLeft = false;
            tappedRight = false;
            doubleTapTime = new float[2];
            _animNumber = (int)AnimNumber.Idle;
        }

    }

    // Perform an airdash
    void airDash()
    {

        // Get currentTime in seconds to compare
        float currentTime = Time.realtimeSinceStartup;

        if (canAirDash)
        {

            // Tapping right
            if (Input.GetKeyDown(Right))
            {
                if (tappedRight)
                {
                    // See if time between taps is quick enough
                    if (currentTime - doubleTapTime[0] < .2f)
                    {
                        if (faceRight)
                            forwardDash();
                        else
                            backDash();
                    }
                    else
                    {
                        // Push previous time back and record new time
                        doubleTapTime[1] = doubleTapTime[0];
                        doubleTapTime[0] = currentTime;
                    }
                }
                else
                {
                    // This is the first time right is tapped
                    tappedLeft = false;
                    doubleTapTime[0] = currentTime;
                }
                tappedRight = true;
            }
            // Tapping left
            else if (Input.GetKeyDown(Left))
            {
                if (tappedLeft)
                {
                    // See if time between taps is quick enough
                    if (currentTime - doubleTapTime[0] < .2f)
                    {
                        if (!faceRight)
                            forwardDash();
                        else
                            backDash();
                    }
                    else
                    {
                        // Push previous time back and record new time
                        doubleTapTime[1] = doubleTapTime[0];
                        doubleTapTime[0] = currentTime;
                    }
                }
                else
                {
                    // This is the first time left is tapped
                    tappedRight = false;
                    doubleTapTime[0] = currentTime;
                }
                tappedLeft = true;
            }

        }

    }

    // Adds velocity to player for dashing forwards
    void forwardDash()
    {
        canAirDash = false;
        airDashing = true;
        _animNumber = (int)AnimNumber.ForwardDash;
        if (faceRight)
            body.velocity = new Vector2(moveSpeed * 8.5f, moveSpeed * 2);
        else
            body.velocity = new Vector2(-moveSpeed * 8.5f, moveSpeed * 2);
        speed.x *= .25f;
    }

    // Adds velocity to player for dashing backwards
    void backDash()
    {
        canAirDash = false;
        airDashing = true;
        _animNumber = (int)AnimNumber.BackDash;
        if (!faceRight)
            body.velocity = new Vector2(moveSpeed * 8f, moveSpeed * 2);
        else
            body.velocity = new Vector2(-moveSpeed * 8f, moveSpeed * 2);
        speed.x *= .25f;
    }

    // Flips the direction the player is facing
    void flipPlayer()
    {
        scale.x *= -1;
        transform.localScale = scale;
        faceRight = !faceRight;
    }

    public float getHealthPercent()
    {
        return health / MAX_HEALTH;
    }

    void attack()
    {
        if (Input.GetKeyDown(Punch) && !isJumping)
        {
            anim.SetTrigger("Punch");
            isAttacking = true;
        }
    }

    void airAttack()
    {

    }
}
