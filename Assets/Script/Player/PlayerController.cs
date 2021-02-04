using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D playerCollider;
    public LayerMask collisionMask;
    public GameObject groundCheck;
    public float groundCheckRadius;

    public float moveSpeed;
    public float jumpForce;

    private float moveAmountX;
    private float moveAmountY;

    private void Start()
    {
       rigidBody = GetComponent<Rigidbody2D>();
       playerCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update() 
    {
        
    }

    private void FixedUpdate() 
    {
        HandleInput();
        HandleMovement();
    }

    bool jump;
    void HandleMovement()
    {
        moveAmountX = rigidBody.velocity.x;
        moveAmountY = rigidBody.velocity.y;
        jump = false;

        isGrounded = GroundCheck();

        //Move
        moveAmountX = moveInputX * moveSpeed;
        //Jump
        if(isGrounded && jumpInput)
        {
            jump = true;
        }
        //Jump Cancel
        if(jumpInputUp && moveAmountY > 0)
        {
            moveAmountY = 0;
        }

        Move(moveAmountX, moveAmountY, jump);
    }

    void Move(float velocityX, float velocityY, bool jump)
    {
        if(jump)
        {
            velocityY += jumpForce;
        }
     
        rigidBody.velocity = new Vector2(velocityX, velocityY);
    }

    public bool isGrounded;
    bool GroundCheck()
    {
        if(Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, collisionMask))
        {
            return true;
        }
        return false;
    }

    void SlopeCheck()
    {  
        Vector2 CheckPos = transform.position - new Vector3(0, playerCollider.size.y / 2);
    }

    void SlopeCheckHorizontal()
    {

    }

    void SlopeCheckVertical()
    {
        
    }

    float moveInputX;
    float moveInputY;
    bool jumpInput;
    bool jumpInputUp;
    void HandleInput()
    {
        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputY = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        jumpInputUp = Input.GetKeyUp(KeyCode.Space);
    }
}
