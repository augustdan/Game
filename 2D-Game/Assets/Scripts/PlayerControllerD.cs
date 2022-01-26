using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    private float movementInputDirection;
    private Rigidbody2D rb;
    private Animator anim;
    private int amountOfJumpsLeft;

    public int amountOfJumps = 1;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;

    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    
    [Header("WallSliding")]
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public Transform wallCheck;
    
    [Header("Ground")]
    public Transform groundCheck;

    public LayerMask whatIsGround;

    [Header("Dashing")]
    public bool canDash = true;
    public float dashTime;
    public float dashSpeed;
    public float dashJumpIncrease;
    public float timeBtwDashes;
    public bool hasDashed = true;


    void Start()
    {
        if (isLocalPlayer == false)
            return;
        
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            amountOfJumpsLeft = amountOfJumps;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        UpdateAnimations();
        CheckIfWallSliding();
        CheckIfCanDash();
        if (Input.GetKeyDown(KeyCode.Mouse1) && movementInputDirection != 0)
        {
            if (hasDashed == true)
            {
                DashAbility();
                anim.SetTrigger("DashTrigger");
                
            } 
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
        
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            amountOfJumpsLeft = amountOfJumps;
            canJump = true;
        }
        if(amountOfJumpsLeft<= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }
    private void CheckIfCanDash()
    {
        if (isGrounded)
        {
            hasDashed = true;
        }
    }

    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) 
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
        
    }
    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);

        if (isWallSliding)
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

   
    private void Flip()
    {
        if (!isWallSliding)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
    private void DashAbility()
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    IEnumerator Dash()
    {
        canDash = false;
        movementSpeed = dashSpeed;
        //jumpForce = dashJumpIncrease;
        yield return new WaitForSeconds(dashTime);
        movementSpeed = 11;
        jumpForce = 18.5f;
        yield return new WaitForSeconds(timeBtwDashes);
        canDash = true;
        if (groundCheck)
        {
            hasDashed = false;
            movementSpeed = 11f;
            jumpForce = 16f;
            
        }
    }
}
