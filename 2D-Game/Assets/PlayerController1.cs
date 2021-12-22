using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerController1 : MonoBehaviour
    {
        private float movementInputDirection;
        private Rigidbody2D rb;
        private int amountOfJumpsLeft;

        public int amountOfJumps = 1;

        private bool isFacingRight = true;
        private bool isGrounded;
        private bool canJump;

        public float movementSpeed = 10.0f;
        public float jumpForce = 16.0f;
        public float groundCheckRadius;

        public Transform groundCheck;

        public LayerMask whatIsGround;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            amountOfJumpsLeft = amountOfJumps;
        }

        // Update is called once per frame
        void Update()
        {
            CheckInput();
            CheckMovementDirection();
            CheckIfCanJump();
        }
        private void FixedUpdate()
        {
            ApplyMovement();
            CheckSurroundings();
        }

        private void CheckSurroundings()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        }

        private void CheckIfCanJump()
        {
            if (isGrounded && rb.velocity.y <= 0)
            {
                amountOfJumpsLeft = amountOfJumps;
                canJump = true;
            }
            if (amountOfJumpsLeft <= 0)
            {
                canJump = false;
            }
            else
            {
                canJump = true;
            }
        }

        private void CheckMovementDirection()
        {
            if (isFacingRight && movementInputDirection < 0)
            {
                Flip();
            }
            else if (!isFacingRight && movementInputDirection > 0)
            {
                Flip();
            }
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
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        private void Flip()
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
