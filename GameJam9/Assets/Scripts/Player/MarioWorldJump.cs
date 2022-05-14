using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioWorldJump : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer, platformLayer;

    [Header("Physics")]
    public float maxSpeed = 4f;
    public float linearDrag;
    public float gravity = 1;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false, onPlatform = false;
    public float groundLength = 0.1f;
    public Vector3 colliderOffset;

    [Header("Animator")]
    private bool idle, damping, isJumping, isWalk;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(rb.velocity.x);
        SetAnimations();
        if (Input.GetAxisRaw("Horizontal") == 0 && !damping)
        {
            idle = true;
            isWalk = false;
        }
        else
        {
            isWalk = true;
            idle = false;
        }

        if (!onGround)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
        }

        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        onPlatform = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, platformLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, platformLayer);

        if (onPlatform)
        {
            onGround = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        moveCharacter(direction.x);

        if (jumpTimer > Time.time && onGround)
        {
            Jump();
        }

        modifyPhysics();
    }

    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
            if (changingDirections)
            {
                //Debug.Log("1 " + rb.drag);
                rb.drag = linearDrag;
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    damping = true;
                }
            }
            /*
            else if(Mathf.Abs(direction.x) < 0.4f && Mathf.Abs(rb.velocity.x) < 0.4f)
            {
                rb.drag = linearDrag;

            }*/
            else if (Mathf.Abs(direction.x) < 0.4f && !Input.GetButton("Jump"))
            {
                rb.drag = 16f;
            }
            else
            {
                rb.drag = 4;
                damping = false;
                //Debug.Log("3 " + rb.drag);
            }
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;

            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2.5f);
            }
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    void SetAnimations()
    {
        animator.SetBool("idle", idle);
        animator.SetBool("damping", damping);
        animator.SetBool("Walk", isWalk);
        animator.SetBool("Jump", isJumping);
        animator.SetFloat("vertical", rb.velocity.y);
        animator.SetFloat("VelY", rb.velocity.y);





    }
}