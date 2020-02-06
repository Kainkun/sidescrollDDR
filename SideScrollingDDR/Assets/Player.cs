using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    Camera mainCamera;

    //input
    Vector2 leftDirection;
    Vector2 rightDirection;

    //movement
    Rigidbody2D rb;
    Vector2 velocity;
    public float moveSpeed = 5;
    public float jumpHeight = 5;
    public float jumpDistance = 5;
    public float timeToJumpHeight = 0.5f;
    float jumpGravity;
    float jumpVelocity;
    public float groundedRaycastBoxWidth = 1;
    public float groundedRaycastDistance = 0.01f;
    bool ceiling;
    bool grounded;



    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        CalculateJumpData();

        grounded = true;
    }


    void Update()
    {
        CalculateJumpData();
        MovementInput();
        ActionInput();
        CheckGrounded();
        CheckCeiling();
        CalculateMovement();
    }

    void MovementInput()
    {
        leftDirection.x = Input.GetAxis("LHorizontal");
        leftDirection.y = Input.GetAxisRaw("LVertical");

        rightDirection.x = Input.GetAxisRaw("RHorizontal");
        rightDirection.y = Input.GetAxisRaw("RVertical");

        if (Input.GetKeyDown(KeyCode.W))
            Jump();
    }

    void ActionInput()
    {
        if (Input.GetButtonDown("FireUp"))
            AttackInDirection(directions.UP);
        if (Input.GetButtonDown("FireDown"))
            AttackInDirection(directions.DOWN);
        if (Input.GetButtonDown("FireRight"))
            AttackInDirection(directions.RIGHT);
        if (Input.GetButtonDown("FireLeft"))
            AttackInDirection(directions.LEFT);
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


    public PolygonCollider2D PolyTrigUp;
    public PolygonCollider2D PolyTrigRight;
    public PolygonCollider2D PolyTrigDown;
    public PolygonCollider2D PolyTrigLeft;

    enum directions {UP, DOWN, RIGHT, LEFT}
    LayerMask enemyOnlyMask = 1 << 8;
    void AttackInDirection(directions direction)
    {

        Vector2[] corners = new Vector2[3];
        corners[0] = mainCamera.transform.InverseTransformPoint(transform.position);
        Vector2 camPos = mainCamera.transform.position;
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D CF = new ContactFilter2D();
        CF.useTriggers = true;
        CF.SetLayerMask(enemyOnlyMask);
        switch (direction)
        {
            case directions.UP:
                corners[1] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(0, 1)) - camPos;
                corners[2] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(1, 1)) - camPos;
                PolyTrigUp.points = corners;
                PolyTrigUp.OverlapCollider(CF, overlappingColliders);
                break;
            case directions.DOWN:
                corners[1] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(1, 0)) - camPos;
                corners[2] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(0, 0)) - camPos;
                PolyTrigDown.points = corners;
                PolyTrigDown.OverlapCollider(CF, overlappingColliders);
                break;
            case directions.RIGHT:
                corners[1] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(1, 1)) - camPos;
                corners[2] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(1, 0)) - camPos;
                PolyTrigRight.points = corners;
                PolyTrigRight.OverlapCollider(CF, overlappingColliders);
                break;
            case directions.LEFT:
                corners[1] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(0, 0)) - camPos;
                corners[2] = (Vector2)mainCamera.ViewportToWorldPoint(new Vector2(0, 1)) - camPos;
                PolyTrigLeft.points = corners;
                PolyTrigLeft.OverlapCollider(CF, overlappingColliders);
                break;
        }

        var closestEnemy = GetClosestEnemy(overlappingColliders);
        if(closestEnemy)
            closestEnemy.GetComponent<Enemy>().TakeDamage(1);

    }

    Collider2D GetClosestEnemy(List<Collider2D> enemies)
    {
        Collider2D bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Collider2D potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }


}
