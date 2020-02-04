using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    Vector2 leftDirection;
    Vector2 rightDirection;

    public float moveSpeed = 5;

    public float groundedRaycastBoxWidth = 1;
    public float groundedRaycastDistance = 0.01f;
    bool ceiling;
    bool grounded;

    Vector2 velocity;

    public float jumpHeight = 5;
    public float jumpDistance = 5;
    public float timeToJumpHeight = 0.5f;

    float jumpGravity;
    float jumpVelocity;

    Rigidbody2D rb;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        CalculateJumpData();

        grounded = true;
    }


    void Update()
    {
        CalculateJumpData();
        GetInput();
        CheckGrounded();
        CheckCeiling();
        CalculateMovement();
    }

    void GetInput()
    {
        leftDirection.x = Input.GetAxis("LHorizontal");
        leftDirection.y = Input.GetAxisRaw("LVertical");

        rightDirection.x = Input.GetAxisRaw("RHorizontal");
        rightDirection.y = Input.GetAxisRaw("RVertical");

        if (Input.GetKeyDown(KeyCode.W))
            Jump();
    }

    void CalculateMovement()
    {
        velocity.x = leftDirection.x * moveSpeed;
        rb.velocity = velocity;
    }

    void CheckGrounded()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(groundedRaycastBoxWidth, 1f), 0, Vector2.down, groundedRaycastDistance))
        {
            if (!grounded)
            {
                grounded = true;
                velocity.y = 0;
            }
        }
        else
        {
            grounded = false;
            velocity.y += jumpGravity * Time.deltaTime;
        }
    }

    void CheckCeiling()
    {
        if (Physics2D.BoxCast(transform.position, new Vector2(groundedRaycastBoxWidth, 1f), 0, Vector2.up, groundedRaycastDistance))
        {
            if (!ceiling)
            {
                ceiling = true;
                velocity.y = 0;
            }
        }
        else
        {
            ceiling = false;
        }
    }

    void CalculateJumpData()
    {
        jumpGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpHeight, 2);
        jumpVelocity = Mathf.Abs(jumpGravity) * timeToJumpHeight;
    }

    void Jump()
    {
        if(grounded)
            velocity.y = jumpVelocity;
    }
}
