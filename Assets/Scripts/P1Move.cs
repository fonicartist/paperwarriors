using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Move : MonoBehaviour {
    // CONSTANTS
    private static float GRAVITY = 9.5f;

    // PUBLIC 
    public GameObject otherPlayer;
    public float moveSpeed = 10;
    public float MAX_HEALTH;
    public float health;
    public bool faceRight;
    public new string name;

    public KeyCode Up, Down, Left, Right, Punch, UpperCut, Aerial;
    
    // PRIVATE
    private float deltaTime, 
                  previousDelta; 
    private float [] doubleTapTime;
    private bool isJumping = false,
                 airDashing = false,
                 canAirDash = false,
                 tappedLeft = false,
                 tappedRight = false,
                 isAttacking,
                 attackFromGround,
                 isGrounded,
                 holdingBack = false;
    private enum AnimNumber : int { Idle, Jumping, ForwardDash, BackDash, Hurt };
    private int _animNumber,
                invincibleFrames,
                attackType;
    private Vector3 pos,
                    otherPos;
    private Vector2 speed,
                    scale;
    private Rigidbody2D body;
    private Animator anim;

    // Use this for initialization
    void Start() {
        speed = new Vector2(0, 0);
        scale = gameObject.transform.localScale;
        doubleTapTime = new float[2];
        body = gameObject.GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        _animNumber = (int)AnimNumber.Idle;
        health = MAX_HEALTH;
        isAttacking = false;
        isGrounded = true;
        attackFromGround = false;
        attackType = -1;
        previousDelta = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update() {

        // Grab references for position and Time for this frame
        pos = transform.position;
        otherPos = otherPlayer.transform.localPosition;
        deltaTime = Time.realtimeSinceStartup - previousDelta;
        previousDelta = Time.realtimeSinceStartup;

        // If time is not paused the rest of update will be executed
        if (Time.timeScale == 1f) {

            // See if player is holding back to block
            isBlocking();

            // Prevent player from clipping through the stage boundaries
            if (isGrounded && speed.y != 0 && pos.y < -1.56f)
                speed.y = 0;
            else if (pos.y > -1.5f && !isGrounded)
                setInAir();
            else if (pos.y < -1.7)
            {
                pos.y = -1.6f;
                if (pos.x < -11.4)
                    pos.x = -11.4f;
                else if (pos.x > 11.3f)
                    pos.x = 11.3f;
                speed = new Vector2(0, 0);
                setGrounded();
                body.velocity = new Vector2(0, 0);
                transform.SetPositionAndRotation(pos, new Quaternion());
            }

            // Actions to do if the player if idling or crouching
            if (idling() || crouching()) {

                // Reset attack capability 
                setAttack(false);

                // Flip player around if needed
                if (faceRight && pos.x > otherPos.x + 0.5f)
                    flipPlayer();
                else if (!faceRight && pos.x < otherPos.x - 0.5f)
                    flipPlayer();
            }

            // Play hurt animation if being attacked
            if (invincibleFrames > 0)
            {
                invincibleFrames--;
                if (invincibleFrames < 15)
                    anim.SetBool("IsHurt", false);
            }

            // Actions to do if player is not in hitstun
            if (!anim.GetBool("IsHurt"))
            {

                // Call the function that controls player movement
                movePlayer();

                // Set the animation to be played
                anim.SetInteger("AnimNumber", _animNumber);

                if (!isAttacking && !landing())
                {
                    // Check for attack input
                    attack();
                    airAttack();
                }

            }
        }
    }

    // Moves the player 
    void movePlayer() {

        // These actions can only be done when not jumping
        if (isGrounded && canWalk() && !jumping()) {

            // Determine Left Right Movement
            if (Input.GetKey(Right))
                speed.x = 1;
            else if (Input.GetKey(Left))
                speed.x = -1;
            else
                speed.x = 0;

            // Call function for handling player jumps
            jump();
        }

        // Check for crouching input
        crouch();

        // Updates jumping motion while isJumping is true
        updateJump();

        // Do not allow horizontal movement when landing or crouching
        if (landing() || crouching())
            speed.x = 0;

        // Carry out motions defined by speed vector
        transform.Translate(speed.x * moveSpeed * deltaTime, speed.y * moveSpeed * deltaTime, 0);
    }

    // Checks for jumping action
    void jump() {

        // Check for jump command
        if (Input.GetKeyDown(Up) && !isAttacking) {
            speed = new Vector2(0, 4);
            setInAir();
            canAirDash = true;
            _animNumber = (int)AnimNumber.Jumping;
            anim.SetTrigger("Jump");

            // Check if jumping forward or backward
            if (Input.GetKey(Left)) { 
                speed.x = -.85f;
            }
            else if (Input.GetKey(Right)) { 
                speed.x = .85f;
            }
        }

    }

    // Updates position when jumping or in the air
    void updateJump() {

        // Check if an air dash is being performed
        if (speed.y > 0 || pos.y > 1.15)
            airDash();

        // Apply gravity
        if (pos.y > -1.6f)
            speed.y -= GRAVITY * deltaTime;

        // Prevent jumping over/on the other player if too close while in the air
        if (pos.y < 2.3 && speed.y < 0 && !airDashing) {
            if (Mathf.Abs(otherPos.x - pos.x) < 0.5f) {
                if (pos.x < otherPos.x + 1)
                    transform.SetPositionAndRotation(new Vector3(otherPos.x - 1f, pos.y, pos.z), new Quaternion());
                else if (pos.x > otherPos.x - 1)
                    transform.SetPositionAndRotation(new Vector3(otherPos.x + 1f, pos.y, pos.z), new Quaternion());
                speed.x = 0;
            }
            else if (Mathf.Abs(otherPos.x - pos.x) < 1.5f) {
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

    }

    // Perform an airdash
    void airDash () {

        // Get currentTime in seconds to compare
        float currentTime = Time.realtimeSinceStartup;

        if (canAirDash) {

            // Tapping right
            if (Input.GetKeyDown(Right)) {
                if (tappedRight) {
                    // See if time between taps is quick enough
                    if (currentTime - doubleTapTime[0] < .2f) {
                        if (faceRight)
                            forwardDash();
                        else
                            backDash();
                    }
                    else { 
                        // Push previous time back and record new time
                        doubleTapTime[1] = doubleTapTime[0];
                        doubleTapTime[0] = currentTime;
                    }
                }
                else {
                    // This is the first time right is tapped
                    tappedLeft = false;
                    doubleTapTime[0] = currentTime;
                }
                tappedRight = true;
            }
            // Tapping left
            else if (Input.GetKeyDown(Left)) {
                if (tappedLeft) {
                    // See if time between taps is quick enough
                    if (currentTime - doubleTapTime[0] < .2f) {
                        if (!faceRight)
                            forwardDash();
                        else
                            backDash();
                    }
                    else {
                        // Push previous time back and record new time
                        doubleTapTime[1] = doubleTapTime[0];
                        doubleTapTime[0] = currentTime;
                    }
                }
                else {
                    // This is the first time left is tapped
                    tappedRight = false;
                    doubleTapTime[0] = currentTime;
                }
                tappedLeft = true;
            }

        }

    }

    // Adds velocity to player for dashing forwards
    void forwardDash () {
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
    void backDash () {
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
    void flipPlayer () {
        scale.x *= -1;
        transform.localScale = scale;
        faceRight = !faceRight;
    }

    // Returns the player's health as a percent
    public float getHealthPercent() {
        return health / MAX_HEALTH;
    }

    // Put's player in a death state to prevent multiple coroutines from being called
    public void setDeathHealth()
    {
        health = -1;
    }

    // Perform grounded attacks
    void attack()
    {
        // Perform lunging attack
        if (Input.GetKeyDown(Punch) && !Input.GetKey(UpperCut) && isGrounded && !attackFromGround) {
            anim.SetTrigger("Punch");
            isAttacking = true;
            attackType = 0;
            speed.x = 0;
            if (faceRight)
                body.velocity = new Vector2(moveSpeed * 4f, body.velocity.y);
            else
                body.velocity = new Vector2(-moveSpeed * 4f, body.velocity.y);
        }
        // Perform anti-air attack
        else if (Input.GetKeyDown(UpperCut) && !Input.GetKey(Punch) && isGrounded && !attackFromGround) {
            anim.SetTrigger("Uppercut");
            isAttacking = true;
            setInAir();
            attackType = 2;
            speed.x = 0;
            speed.y = .5f;
            if (faceRight)
                body.velocity = new Vector2(moveSpeed * 2.5f, body.velocity.y + moveSpeed * 6.5f);
            else
                body.velocity = new Vector2(-moveSpeed * 2.5f, body.velocity.y + moveSpeed * 6.5f);
        }
    }

    // Perform aerial attack
    void airAttack() {
        if (Input.GetKeyDown(Aerial) && isJumping && pos.y > 2.1f || (attackFromGround && pos.y > 2.7f)) {
            anim.SetTrigger("Dive");
            isAttacking = true;
            attackFromGround = false;
            attackType = 1;
            speed.x = 0;
            speed.y = -1f;
            if (faceRight) { 
                body.velocity = new Vector2(moveSpeed * 6.5f, 0); 
                speed.x = 1f; 
            }
            else {
                body.velocity = new Vector2(-moveSpeed * 6.5f, 0);
                speed.x = -1f;
            }
        }
        else if (Input.GetKeyDown(Aerial) && isGrounded && !isAttacking) {
            speed = new Vector2(0, 5);
            setInAir();
            anim.SetTrigger("Jump");
            _animNumber = (int)(AnimNumber.Jumping);
            canAirDash = false;
            attackFromGround = true;
        }
    }

    // Checks if currently attacking
    public bool getAttack() {
        return isAttacking;
    }

    // Set value of isAttacking variable
    public void setAttack (bool val) {
        if (val == false)
            attackType = -1;
        isAttacking = false;
    }

    // Player is hurt by an attack
    public void getHurt () {
        if (health > 10)
            health -= 10;
        else
            health = 0;

        speed = new Vector2(0, 0);
        body.velocity = new Vector2(0, 0);

        invincibleFrames = 27;
        anim.SetBool("IsHurt", true);
        if (pos.y > -.5f)
            hitInAir();
        _animNumber = (int)AnimNumber.Idle;
    }

    // Player collides with the ground
    public void setGrounded() {
        isGrounded = true;
        isJumping = false; 
        canAirDash = false;
        tappedLeft = false;
        tappedRight = false;
        doubleTapTime = new float[2];
        speed.y = 0;
        body.velocity = new Vector2(body.velocity.x, -5);
        _animNumber = (int)AnimNumber.Idle;
        anim.SetBool("IsGrounded", true);
    }

    // Gets player's status of being grounded
    public bool getGrounded(){
        return isGrounded;
    }

    // Player is in the air
    public void setInAir() {
        isJumping = true;
        isGrounded = false;
        anim.SetBool("IsGrounded", false);
    }

    // Player is hit in the air
    public void hitInAir() {
        isJumping = false;
        isGrounded = false;
        anim.SetBool("IsGrounded", false);
    }

    // Returns the current attack type
    public int getAttackType() {
        return attackType;
    }

    // Returns the player's invincibilty status
    public bool isInvincible() {
        return invincibleFrames > 0;
    }

    // Coroutine to wait for ingame seconds
    public IEnumerator wait(int seconds) {
        yield return new WaitForSeconds(seconds);
    }

    // Knocks the player away from opponent
    public void flyback() {
        body.velocity = new Vector2(-100, 5);
        if (!faceRight)
            body.velocity *= new Vector2(-1, 1);
    }

    // Checks to see if player is blocking
    public bool isBlocking () {
        if (dashing())
            holdingBack = false;
        else if ((faceRight && Input.GetKey(Left)) || (!faceRight && Input.GetKey(Right)))
            holdingBack = true;
        else
            holdingBack = false;
        return holdingBack;
    }

    // Chip damage to take away if player is blocking
    public void block () {
        if (health > 0)
            health -= 2;
    }
    
    // 
    void crouch () {
        if (isGrounded && Input.GetKey(Down))
            anim.SetBool("HoldingDown", true);
        else
            anim.SetBool("HoldingDown", false);
    }

    bool canWalk () {
        return
            !isAttacking && !isJumping &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Punch_Anim") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Upper_Anim") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Anim") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Landing_Anim");
    }

    bool idling () {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    bool landing () {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Landing_Anim");
    }

    bool crouching () {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Crouch_Anim");
    }

    bool jumping () {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");
    }

    bool dashing () {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName("Forward_Dash") || anim.GetCurrentAnimatorStateInfo(0).IsName("Back_Dash"));
    }

}
