using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Metadata;

public class MainPlayerMovement : MonoBehaviour // player code
{
    public Rigidbody2D playerRB;
    public BoxCollider2D playerCOLL;
    public SpriteRenderer playerSP;
    public Transform playerTRANS;
    public Transform endTrigTRANS;
    public LayerMask environmentLayerMask; // Layer for boxcast to hit
    public GameObject juiceVacuum;
    public ParticleSystem playerGroundSamsh;
    public GameObject onSplash;

    public Vector3 idleScaleChange; // The unit of change idle animation 
    public Vector3 curNormScale; // The current normal scale of the player
    public Vector3 maxNormScale;
    public Vector3 minNormScale;
    public Vector3 chargeFullScale; // The total scale player squish up to

    public float jumpForce; // How hard does the player gets launched 
    public float chargeTime; // How long has the player been charging
    public float maxJumpForce;
    public float minJumpForce;
    public float maxChargeTime;
    public float xSpeed; // To limit horizontal movement
    public float idleScaleSpeed; // How fast should the idle animation change
    public float reboundScaleSpeed; // Same thing for rebounding
    public float juiceAmount;
    public float suckSpeed; //  Suck juice per second
    public float decreasePerJump;
    public float groundCheckTimer;

    public bool charging; // Charging or not
    public bool rebounding; // Rebounding or not
    public bool grounded;
    public bool groundSub1;
    public bool groundSub2;
    public bool facingRight;
    public bool onJuice; // If the player is on a juice source
    public bool onTempJuice; // If the player is on a temp pud of juice that can be sucked back
    public bool canDecrease; // To prevent the player from keep decreasing in size

    public int jumpingDir; // 1 = right, -1 = left, 0 = static

    public char collFloorDir; // The direction the player collides with the floor (R, L, U, D) (N = NULL)

    public UnityEngine.KeyCode JumpKey;
    public UnityEngine.KeyCode LeftKey;
    public UnityEngine.KeyCode RightKey;
    public UnityEngine.KeyCode SuckKey;

    // Use this for initialization
    void Awake()
    {
        try
        {
            JumpKey = SettingsUILogic.instance.jumpKey;
            LeftKey = SettingsUILogic.instance.leftKey;
            RightKey = SettingsUILogic.instance.rightKey;
            SuckKey = SettingsUILogic.instance.interactKey;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Settings from UI cannot be fetched!");
            Debug.LogWarning("Settings are now on default");
            Debug.LogWarning("jump : space bar");
            Debug.LogWarning("left : A");
            Debug.LogWarning("Right : D");
            Debug.LogWarning("interact : S");
            JumpKey = KeyCode.Space;
            LeftKey = KeyCode.A;
            RightKey = KeyCode.D;
            SuckKey = KeyCode.S;
        }


        playerTRANS = GetComponent<Transform>();
        playerCOLL = GetComponent<BoxCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
        playerSP = GetComponent<SpriteRenderer>();
        playerGroundSamsh = GetComponentInChildren<ParticleSystem>();
        juiceVacuum = transform.Find("Juice collector").gameObject;
        juiceVacuum.SetActive(false);
        environmentLayerMask = LayerMask.GetMask("Ground");
        endTrigTRANS = GameObject.FindGameObjectWithTag("END").GetComponent<Transform>();

        charging = false;
        grounded = false;
        jumpForce = 0;
        maxJumpForce = 70;
        minJumpForce = 20;
        maxChargeTime = 1f;
        xSpeed = 6f;
        facingRight = true;
        idleScaleChange = new Vector3(-0.01f, -0.01f, 0);
        idleScaleSpeed = 25f;
        chargeFullScale = new Vector3(1.1f, 0.4f, 1f);
        rebounding = false;
        reboundScaleSpeed = 20;
        curNormScale = new Vector3(1f, 1f, 1f);
        minNormScale = new Vector3(0.5f, 0.5f, 0.5f);
        maxNormScale = new Vector3(1.6f, 1.6f, 1.6f);
        juiceAmount = 70f;
        suckSpeed = 50f;
        decreasePerJump = 5f;
        canDecrease = false;
        

        float spawnY;
        if( PlayerPrefs.GetString("PlayerDirection") == "out")
        {
            spawnY = -9.1f;
        }
        else
        {
            spawnY = endTrigTRANS.position.y + 0.5f;
        }
        juiceAmount = PlayerPrefs.GetFloat("PlayerJuice");
        playerRB.velocity = new Vector2(PlayerPrefs.GetFloat("PlayerVelocityX"), PlayerPrefs.GetFloat("PlayerValocityY"));
        playerTRANS.position = new Vector3(PlayerPrefs.GetFloat("PlayerPositionX", playerTRANS.position.x), spawnY, 0);
        // Map the current normal scale according to the juice amount
        curNormScale.x = Map(0f, 100f, minNormScale.x, maxNormScale.x, juiceAmount);
        curNormScale.y = Map(0f, 100f, minNormScale.y, maxNormScale.y, juiceAmount);
        curNormScale.z = Map(0f, 100f, minNormScale.z, maxNormScale.z, juiceAmount);
        playerTRANS.localScale = curNormScale;

    }
    // Update is called once per frame for physics
    void Update()
    {
        // Map the current normal scale according to the juice amount
        curNormScale.x = Map(0f, 100f, minNormScale.x, maxNormScale.x, juiceAmount);
        curNormScale.y = Map(0f, 100f, minNormScale.y, maxNormScale.y, juiceAmount);
        curNormScale.z = Map(0f, 100f, minNormScale.z, maxNormScale.z, juiceAmount);
        // Map the juice particle size according to the juice amount
        playerGroundSamsh.startSize = Map(0f, 100f, 0.3f, 0.85f, juiceAmount);
        
        grounded = CheckGrounded();

        if (grounded)
        {
            GrowHandler();
            ChargeHandler(); // Checks if the player is charging
            if (charging)
            {
                ChargeAnimation();
            }
            else if (rebounding)
            {
                ReboundAnimation();
            }
            else IdleAnimation();

            JumpHandler(); // Scale decreases scale when player jump, also locks the player's in-air direction
            FlipHandler(); 
        }
        if (!grounded)
        {
            juiceVacuum.SetActive(false);
            if (rebounding)
            {
                ReboundAnimation();
            }

            InAirMovementHandler(jumpingDir);
        }

    }
    
    void GrowHandler()
    {
        if (Input.GetKey(SuckKey) && juiceAmount < 100f)
        {
            juiceVacuum.SetActive(true);
        }
        else juiceVacuum.SetActive(false);

        if (onJuice)
        {
            if (Input.GetKey(SuckKey) && juiceAmount < 100f)
            {
                juiceAmount += (suckSpeed * Time.deltaTime);
            }
        }
    }
    void ChargeAnimation()
    {
        chargeFullScale.x = curNormScale.x * 1.1f; chargeFullScale.y = curNormScale.y * 0.4f; chargeFullScale.z = curNormScale.z * 1f;
        playerTRANS.localScale = Vector3.Slerp(playerTRANS.localScale, chargeFullScale, chargeFullScale.y / maxChargeTime * Time.deltaTime);
    }
    void ReboundAnimation()
    {
        if (playerTRANS.localScale == curNormScale)
        {
            rebounding = false;
        }
        playerTRANS.localScale = Vector3.Slerp(playerTRANS.localScale, curNormScale, reboundScaleSpeed * Time.deltaTime);
    }
    void IdleAnimation()
    {
        if (playerTRANS.localScale.y <= curNormScale.y)
        {
            idleScaleChange = new Vector3(0.01f, 0.01f, 0);
        }
        if (playerTRANS.localScale.y >= curNormScale.y*1.1f)
        {
            idleScaleChange = new Vector3(-0.01f, -0.01f, 0);
        }
        playerTRANS.localScale += idleScaleChange * idleScaleSpeed * Time.deltaTime;
    }
    void InAirMovementHandler(int jumpingDir)
    {
        if (jumpingDir == 1) // jumping to the right
        {
            if (xSpeed < 0)
            {
                xSpeed = 0;
                playerRB.velocity = new Vector2(0, playerRB.velocity.y);
            }
            else if (playerRB.velocity.x != xSpeed)
            {
                playerRB.velocity = new Vector2(xSpeed, playerRB.velocity.y);
            }
        }
        else if (jumpingDir == -1) // jumping to the left
        {
            if (xSpeed < 0)
            {
                xSpeed = 0;
                playerRB.velocity = new Vector2(0, playerRB.velocity.y);
            }
            else if (playerRB.velocity.x != -xSpeed)
            {
                playerRB.velocity = new Vector2(-xSpeed, playerRB.velocity.y);
            }
        } 
    }
    void FlipHandler()
    {
        if (Input.GetKey(RightKey))
        {
            playerSP.flipX = false;
            facingRight = true;
        }
        if (Input.GetKeyDown(LeftKey))
        {
            playerSP.flipX = true;
            facingRight = false;
        }
    }
    void ChargeHandler()
    {
        if (Input.GetKey(JumpKey) && !charging)// If the plauer is grounded and it is the first time jump key changes to presssed, start charging
        {
            // Starting the jump charge and set charge time to zero, also stop rebounding
            charging = true;
            chargeTime = 0f;
            rebounding = false;
        }
        else if (charging && Input.GetKey(JumpKey)) // Keep track of the charging time
        {
            chargeTime += Time.deltaTime;
            if (chargeTime < maxChargeTime)
            {
                jumpForce = chargeTime * ((maxJumpForce - minJumpForce) / maxChargeTime) + minJumpForce;
            }
        }
    }
    bool CheckGrounded()
    {
        //if(collFloorDir == 'D' && Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask))
        //{
        //    groundSub1 = true;
        //    groundCheckTimer = 0f;
        //}
        if (!grounded && collFloorDir == 'D' && Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask))
        {
            // landed
            LandHandler();
        }
        
        return (collFloorDir == 'D' && Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask));
    }
    void LandHandler()
    {
        if (juiceAmount > 0f && canDecrease)
        {
            playerGroundSamsh.Play();
            canDecrease = false;
            juiceAmount -= decreasePerJump;
            CreateSplash(); // makes the little puddle of juice when landing
        }
        jumpForce = 0;
        jumpingDir = 0;
        playerRB.velocity = new Vector2(0, playerRB.velocity.y);
    }
    void CreateSplash()
    {
        playerGroundSamsh.Play();
    }
    void JumpHandler() // the function for jumping 
    {
        if (Input.GetKey(LeftKey))
        {
            jumpingDir = -1;
        }
        else if (Input.GetKey(RightKey))
        {
            jumpingDir = 1; 
        }
        else { jumpingDir = 0; }

        if (Input.GetKey(JumpKey) == false && charging && chargeTime > 0.06) // If the player is currently charging but the jump key is released, jump
        {
            xSpeed = 6f;
            grounded = false;
            charging = false;
            chargeTime = 0;
            collFloorDir = 'N';
            rebounding = true;
            canDecrease = true;
            playerTRANS.position = new Vector3(playerTRANS.position.x, playerTRANS.position.y + 0.3f, playerTRANS.position.z);

            if (jumpForce > minJumpForce + 0.04f)
            {
                playerRB.AddForce(Vector2.up * jumpForce * curNormScale.x, ForceMode2D.Impulse);
            }
            else playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" )
        {
            if (Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask))
            {
                collFloorDir = 'D';
                Debug.Log("The ground is below me.");
            }
            else if (Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.up, 0.05f, environmentLayerMask))
            {
                collFloorDir = 'U';
                if (playerRB.velocity.y >0)
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, -playerRB.velocity.y);
                }
            }
            else if (Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.left, 0.05f, environmentLayerMask))
            {
                collFloorDir = 'L';
                //playerTRANS.position = new Vector3(playerTRANS.position.x + 0.1f, playerTRANS.position.y, playerTRANS.position.z);
                jumpingDir = 1;
                xSpeed -= 2;
                Debug.Log("The ground is on my left.");
            }
            else if (Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.right, 0.05f, environmentLayerMask))
            {
                collFloorDir = 'R';
                //playerTRANS.position = new Vector3(playerTRANS.position.x - 0.1f, playerTRANS.position.y, playerTRANS.position.z);
                jumpingDir = -1;
                xSpeed -= 2;
                Debug.Log("The ground is on my right.");
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask))
        {
            collFloorDir = 'N';
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Juice") onJuice = true; 
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Juice") onJuice = false;
    }
    public float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}