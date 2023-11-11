using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MainPlayerMovement : MonoBehaviour // player code
{
    public Rigidbody2D playerRB;
    public BoxCollider2D playerCOLL;
    public SpriteRenderer playerSP;
    public Transform playerTRANS;
    public LayerMask environmentLayerMask; // Layer for boxcast to hit

    public Vector3 idleScaleChange; // The unit of change idle animation 
    public Vector3 chargeFullScale; // The total scale player squish up to

    public float jumpForce; // How hard does the player gets launched 
    public float chargeTime; // How long has the player been charging
    public float maxJumpForce;
    public float minJumpForce;
    public float maxChargeTime;
    public float movementScalar;
    public float maxSpeed; // To limit horizontal movement
    public float idleScaleSpeed; // How fast should the idle animation change
    public float reboundScaleSpeed; // Same thing for rebounding

    public bool charging; // charging or not
    public bool rebounding; // rebounding or not
    public bool grounded;
    public bool facingRight;
    public bool Onjuice;
    public int jumpingDir; // 1 = right, -1 = left, 0 = static

    public UnityEngine.KeyCode JumpKey;
    public UnityEngine.KeyCode LeftKey;
    public UnityEngine.KeyCode RightKey;

    // Use this for initialization
    void Awake()
    {
        playerTRANS = GetComponent<Transform>();
        playerCOLL = GetComponent<BoxCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
        playerSP = GetComponent<SpriteRenderer>();
        jumpForce = 0;
        charging = false;
        grounded = false;
        maxJumpForce = 55;
        minJumpForce = 20;
        maxChargeTime = 0.7f;
        movementScalar = 5f;
        maxSpeed = 10f;
        environmentLayerMask = LayerMask.GetMask("Ground");
        JumpKey = KeyCode.Space;
        LeftKey = KeyCode.A;
        RightKey = KeyCode.D;
        facingRight = true;
        idleScaleChange = new Vector3(-0.01f, -0.01f, 0);
        idleScaleSpeed = 25f;
        chargeFullScale = new Vector3(1.1f, 0.4f, 0);
        rebounding = false;
        reboundScaleSpeed = 20;
    }


    // Update is called once per frame for physics
    void Update()
    {
        grounded = CheckGrounded();
        if (grounded)
        {
            ChargeHandler();
            if (charging)
            {
                ChargeAnimation();
            }
            else if (rebounding)
            {
                ReboundAnimation();
            }
            else IdleAnimation();
            JumpHandler();
            FlipHandler();
        }
        if (!grounded)
        {
            ReboundAnimation();
            InAirMovementHandler(jumpingDir);
        }
    }

    void ChargeAnimation()
    {
        playerTRANS.localScale = Vector3.Slerp(playerTRANS.localScale, chargeFullScale, chargeFullScale.y / maxChargeTime * Time.deltaTime);
    }

    void ReboundAnimation()
    {
        Vector3 norm = new Vector3(1f, 1f, 1f);
        if (playerTRANS.localScale == norm)
        {
            rebounding = false;
        }
        playerTRANS.localScale = Vector3.Slerp(playerTRANS.localScale, norm, reboundScaleSpeed * Time.deltaTime);
    }

    void IdleAnimation()
    {
        if (playerTRANS.localScale.y <= 1.0f)
        {
            idleScaleChange = new Vector3(0.01f, 0.01f, 0);
        }
        if (playerTRANS.localScale.y >= 1.1f)
        {
            idleScaleChange = new Vector3(-0.01f, -0.01f, 0);
        }
        playerTRANS.localScale += idleScaleChange * idleScaleSpeed * Time.deltaTime;
    }

    void InAirMovementHandler(int jumpingDir)
    {
        if (jumpingDir == 1)
        {
            if (playerRB.velocity.magnitude < maxSpeed)
            {
                Vector2 movement = new Vector2(jumpingDir, 0);
                playerRB.AddForce(movementScalar * movement);
            }
        }
        else if (jumpingDir == -1)
        {
            if (playerRB.velocity.magnitude < maxSpeed)
            {
                Vector2 movement = new Vector2(jumpingDir, 0);
                playerRB.AddForce(movementScalar * movement);

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
        // If the plauer is grounded and it is the first tiem jump key changes to presssed, start charging
        if (Input.GetKeyDown(JumpKey))
        {
            // Starting the jump charge and set charge time to zero, also stop rebounding
            charging = true;
            chargeTime = 0;
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
        if (!grounded && Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask))
        {
            jumpForce = 0;
            jumpingDir = 0;
        }
        return Physics2D.BoxCast(playerCOLL.bounds.center, playerCOLL.bounds.size, 0f, Vector2.down, 0.05f, environmentLayerMask);
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
        if (Input.GetKeyUp(JumpKey)) // If the player is currently charging but the jump key is released, jump
        {
            grounded = false;
            charging = false;
            chargeTime = 0;
            if (jumpForce > 0)
            {
                playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            rebounding = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.name == "Juice") Onjuice = true;
        else Onjuice = false;
    }
}