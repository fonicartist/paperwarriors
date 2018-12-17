using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // CONSTANTS
    private static float GRAVITY = 9.5f;

    // PUBLIC 
    public GameObject otherPlayer;
    public float moveSpeed = 10;
    public float MAX_HEALTH;
    public float health;
    public bool faceRight;
    public string playername;
    public int classType;

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
    private enum ClassNumber : int { Fighter, Swordsman, Mage };
    private int _animNumber,
                _classNumber,
                invincibleFrames,
                attackType,
                floatingCounter;
    private Vector3 pos,
                    otherPos;
    private Vector2 speed,
                    scale;
    private Rigidbody2D body;
    private Animator anim;
    private GameObject[] players;

    // Use this for initialization
    void Start() {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
            if (!p.GetComponent<PlayerController>().playername.Equals(playername))
                otherPlayer = p;
        speed = new Vector2(0, 0);
        scale = gameObject.transform.localScale;
        doubleTapTime = new float[2];
        body = gameObject.GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        _animNumber = (int)AnimNumber.Idle;
        _classNumber = classType;
        health = MAX_HEALTH;
        isAttacking = false;
        isGrounded = true;
        attackFromGround = false;
        attackType = -1;
        previousDelta = Time.realtimeSinceStartup;
        setGrounded();
        floatingCounter = 0;
    }

    // Update is called once per frame
    void Update() {

        if (players.Length < 2)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
                if (!p.GetComponent<PlayerController>().playername.Equals(playername))
                    otherPlayer = p;
        }

        if (body == null)
            gameObject.GetComponent<Rigidbody2D>();

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
                if (invincibleFrames < 5)
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
                }
                // Check again for airAttack
                if (!isAttacking && !landing())
                {
                    // Check for attack input
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
            { 
                speed.x = 1;
                if (faceRight)
                    anim.SetBool("Walking", true);
                else
                    anim.SetBool("Reversing", true);
            }
            else if (Input.GetKey(Left))
            {
                speed.x = -1;
                if (!faceRight)
                    anim.SetBool("Walking", true);
                else
                    anim.SetBool("Reversing", true);
            }
            else
            {
                speed.x = 0;
                anim.SetBool("Walking", false);
                anim.SetBool("Reversing", false);
            }


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
            FindObjectOfType<AudioManager>().play("Jump");
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
            GetComponentInChildren<EffectSpawner>().jumpDust();
        }

    }

    // Updates position when jumping or in the air
    void updateJump() {

        // Check if an air dash is being performed
        if (speed.y > 0 || pos.y > 1.15)
            airDash();

        // Apply gravity
        if (pos.y > -1.6f)
        {
            if (getAttack() && classType == 2 && floatingCounter > 0)
            {
                speed.y -= GRAVITY * deltaTime * .2f;
                print("Applying less gravity");
                floatingCounter--;
            }
            else
            {
                speed.y -= GRAVITY * deltaTime;
            }
        }

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
        FindObjectOfType<AudioManager>().play("Dash");
        GetComponentInChildren<EffectSpawner>().dash(0);
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
        FindObjectOfType<AudioManager>().play("Dash");
        GetComponentInChildren<EffectSpawner>().dash(1);
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
        if (health / MAX_HEALTH < .01) 
            return 0;
        else
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
        if (Input.GetKeyDown(Punch) && !Input.GetKey(UpperCut) && isGrounded && !attackFromGround && !isAttacking) {
            switch (_classNumber)
            {
                case 0: FindObjectOfType<AudioManager>().play("Punch");
                    break;
                case 1: FindObjectOfType<AudioManager>().play("Swing");
                    break;
                case 2: FindObjectOfType<AudioManager>().play("Punch");
                    break;
            }
            
            anim.SetTrigger("Punch");
            isAttacking = true;
            attackType = 0;
            speed.x = 0;
            if (_classNumber != 2)
            {
                if (faceRight)
                    body.velocity = new Vector2(moveSpeed * 4f, body.velocity.y);
                else
                    body.velocity = new Vector2(-moveSpeed * 4f, body.velocity.y);
            }
            else
            {
                GetComponentInChildren<EffectSpawner>().castFirebolt();
                if (faceRight)
                    body.velocity = new Vector2(moveSpeed * 2f, body.velocity.y);
                else
                    body.velocity = new Vector2(-moveSpeed * 2f, body.velocity.y);
            }
        }
        // Perform anti-air attack
        else if (Input.GetKeyDown(UpperCut) && !Input.GetKey(Punch) && isGrounded && !attackFromGround && !isAttacking) {
            switch(_classNumber)
            {
                case 0: FindObjectOfType<AudioManager>().play("HeavyKick");
                    FindObjectOfType<AudioManager>().play("Jump");
                    break;
                case 1: FindObjectOfType<AudioManager>().play("HeavySwing");
                    break;
                case 2: FindObjectOfType<AudioManager>().play("HeavyKick");
                    break;
            }
            anim.SetTrigger("Uppercut");
            isAttacking = true;
            attackType = 2;
            speed.x = 0;
            switch (_classNumber) {
                case 0:
                    setInAir();
                    speed.y = .5f;
                    if (faceRight)
                        body.velocity = new Vector2(moveSpeed * 2.5f, body.velocity.y + moveSpeed * 6.5f);
                    else
                        body.velocity = new Vector2(-moveSpeed * 2.5f, body.velocity.y + moveSpeed * 6.5f);
                    break;
                case 1:
                    if (faceRight)
                        body.velocity = new Vector2(moveSpeed * 2.5f, 0);
                    else
                        body.velocity = new Vector2(-moveSpeed * 2.5f, 0);
                    break;
                case 2:
                    GetComponentInChildren<EffectSpawner>().castStalagmite();
                    if (faceRight)
                        body.velocity = new Vector2(moveSpeed * 2f, body.velocity.y);
                    else
                        body.velocity = new Vector2(-moveSpeed * 2f, body.velocity.y);
                    break;

            }
        }
    }

    // Perform aerial attack
    void airAttack() {
        if (Input.GetKeyDown(Aerial) && isJumping && pos.y > 2.1f || 
            (attackFromGround && pos.y > 2.7f) ||
            (Input.GetKeyDown(Aerial) && isJumping && pos.y > .8f && _classNumber == 1)) {
            anim.SetTrigger("Dive");
            isAttacking = true;
            attackFromGround = false;
            attackType = 1;

            switch (_classNumber) { 
                // Martial Artist will Dive Kick
                case (int)ClassNumber.Fighter:
                    speed.y = -.5f;
                    FindObjectOfType<AudioManager>().play("HeavyKick");
                    if (faceRight) { 
                        body.velocity = new Vector2(moveSpeed * 6.5f, 0); 
                        speed.x = 1f; 
                    }
                    else {
                        body.velocity = new Vector2(-moveSpeed * 6.5f, 0);
                        speed.x = -1f;
                    }
                    break;
                // Swordsman will Aerial Slash
                case (int)ClassNumber.Swordsman:
                    FindObjectOfType<AudioManager>().play("Swing");
                    speed.y = .4f;
                    if (faceRight) {
                        body.velocity = new Vector2(moveSpeed * .5f, 0);
                        speed.x = .5f;
                    }
                    else {
                        body.velocity = new Vector2(-moveSpeed * .5f, 0);
                        speed.x = -.5f;
                    }
                    break;
                // Mage summons lightning
                case (int)ClassNumber.Mage:
                    floatingCounter = 7;
                    FindObjectOfType<AudioManager>().play("Punch");
                    GetComponentInChildren<EffectSpawner>().castLightning();
                    speed.y = .4f;
                    body.velocity = new Vector2(0, 1);
                    if (faceRight)
                        speed.x = .8f;
                    else
                        speed.x = -.8f;
                    break;
            }

        }
        else if (Input.GetKeyDown(Aerial) && isGrounded && !isAttacking) {
            FindObjectOfType<AudioManager>().play("Jump");
            GetComponentInChildren<EffectSpawner>().jumpDust();
            if (_classNumber == (int)ClassNumber.Fighter)
                speed = new Vector2(0, 5);
            else
                speed = new Vector2(0, 4);
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
    public void getHurt (int damage) {
        if (health > damage)
            health -= damage;
        else
            health = 0;

        if (health < 2)
            health = 0;

        // Cancel projectile animation
        //if (classType == 2)
            //GetComponentInChildren<EffectSpawner>().cancel();

        // Spawn a hitspark
        PlayerController other = otherPlayer.GetComponent<PlayerController>();
        hitspark(other.getAttackType(), other.classType);

        speed = new Vector2(0, 0);
        body.velocity = new Vector2(0, 0);

        invincibleFrames = 17;
        anim.SetBool("IsHurt", true);
        if (pos.y > -.5f)
            hitInAir();
        _animNumber = (int)AnimNumber.Idle;
    }

    // Spawn appropriate hitspark based on the other player's attack
    void hitspark(int otherAttack, int otherClass)
    {
        print(otherAttack + " " + otherClass);

        
        switch (otherClass)
        {
            // Other player is a Fighter
            case 0:
                switch (otherAttack)
                {
                    // Forward Attack
                    case 0: GetComponentInChildren<EffectSpawner>().hitspark(4);
                        break;
                    // Aerial
                    case 1: GetComponentInChildren<EffectSpawner>().hitspark(1);
                        break;
                    // Anti-Air
                    case 2: GetComponentInChildren<EffectSpawner>().hitspark(2);
                        break;
                }
                break;
            // Other player is a Swordsman
            case 1:
                switch (otherAttack)
                {
                    // Forward Attack
                    case 0: GetComponentInChildren<EffectSpawner>().hitspark(3);
                        break;
                    // Aerial
                    case 1: GetComponentInChildren<EffectSpawner>().hitspark(5);
                        break;
                    // Anti-Air
                    case 2: GetComponentInChildren<EffectSpawner>().hitspark(5);
                        break;
                }
                break;
            // Other player is a Mage
            case 2:
                if (otherAttack == 1)
                    GetComponentInChildren<EffectSpawner>().hitspark(3); // Lightning Spell
                else if (otherAttack == 2)
                    GetComponentInChildren<EffectSpawner>().hitspark(5); // Rock Spell
                break;
        }
        

    }

    // Player collides with the ground
    public void setGrounded() {
        if (!isGrounded)
        {
            FindObjectOfType<AudioManager>().play("Land");
            GetComponentInChildren<EffectSpawner>().landDust();
        }
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

    public int getPower()
    {
        return 10 - _classNumber;
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
        GetComponentInChildren<EffectSpawner>().block();
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
