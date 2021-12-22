using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    PhotonView view;
    public float speed;
    public float JumpForce;

    private Rigidbody2D rb2D;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJumps;
    public int extraJumpValue;

    private bool facingRight;
    void Start()
    {
        view = GetComponent<PhotonView>();

        extraJumps = extraJumpValue;
        rb2D = GetComponent<Rigidbody2D>();
    }

    
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        
            float horizontal = Input.GetAxis("Horizontal");
            //rb2D.velocity = new Vector2(speed * horizontal ,rb2D.velocity.y);
            Vector2 position = transform.position;
            position.x = position.x + speed * horizontal;
            transform.position = position;

            if (facingRight == false && horizontal < 0)
            {
                Flip();
            }
            else if (facingRight == true && horizontal > 0)
            {
                Flip();
            }
        }
    }
    private void Update()
    {
        if (view.IsMine) 
        { 
        if (isGrounded == true)
        {
            extraJumps = 2;
        }
        if (Input.GetButtonDown("Jump") && extraJumps > 0)
        {
            rb2D.velocity = Vector2.up * JumpForce;
            extraJumps--;
        }
        else if (Input.GetButtonDown("Jump") && extraJumps == 0 && isGrounded == true)
        {
            rb2D.velocity = Vector2.up * JumpForce;
        }
    }
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
