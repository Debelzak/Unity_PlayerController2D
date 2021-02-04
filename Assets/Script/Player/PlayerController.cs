using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D playerCollider;
    public LayerMask collisionMask;
    public GameObject groundCheck;
    public PhysicsMaterial2D fullfriction;
    public PhysicsMaterial2D nofriction;

    public float groundCheckRadius;
    public float moveSpeed;
    public float jumpForce;
    public bool canJump;
    public bool isJumping;
    private float moveAmountX;
    private float moveAmountY;

    public float slopeCheckDistance;
    private float slopeSideAngle;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private Vector2 slopeNormalPerpendicular;
    public bool isOnSlope;

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
        GroundCheck();
        SlopeCheck();
        HandleMovement();
    }

    bool jump;
    void HandleMovement()
    {
        moveAmountX = rigidBody.velocity.x;
        moveAmountY = rigidBody.velocity.y;

        jump = false;

        if(isGrounded && !isOnSlope && !isJumping)
        {
            moveAmountX = moveInputX * moveSpeed;
            moveAmountY = 0;
        }
        else if(isGrounded && isOnSlope && !isJumping)
        {
            moveAmountX = moveSpeed * slopeNormalPerpendicular.x * -moveInputX;
            moveAmountY = moveSpeed * slopeNormalPerpendicular.y * -moveInputX;
        }
        else if(!isGrounded)
        {
            moveAmountX = moveInputX * moveSpeed;
        }

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
        if(canJump && jump)
        {
            velocityY = jumpForce;
            isJumping = true;
            canJump = false;
        }

        rigidBody.velocity = new Vector2(velocityX, velocityY);
    }

    public bool isGrounded;
    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, collisionMask);

        if(rigidBody.velocity.y <= 0)
        {
            isJumping = false;
        }

        if(isGrounded && !isJumping)
        {
            canJump = true;
        }
    }

    void SlopeCheck()
    {  
        Vector2 checkPos = transform.position - new Vector3(0, playerCollider.size.y / 2);
        SlopeCheckVertical(checkPos);
        //SlopeCheckHorizontal(checkPos);
    }

    void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, collisionMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, collisionMask);

        if(slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if(slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0;
            isOnSlope = false;
        }
    }  

    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, collisionMask);
        if(hit)
        {
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            
            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, hit.normal, Color.red);
            Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.red);
        }

        if(isOnSlope && moveInputX == 0)
        {
            rigidBody.sharedMaterial = fullfriction;
        }
        else
        {
            rigidBody.sharedMaterial = nofriction;
        }
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
