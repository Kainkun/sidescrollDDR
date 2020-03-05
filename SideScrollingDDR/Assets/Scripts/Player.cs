using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public int jumpCount = 1;
    int currentJumpCount;
    public float jumpHeight = 5;
    public float jumpDistance = 5;
    public float timeToJumpHeight = 0.5f;
    public float coyoteJumpTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    bool jumpBuffer;
    public float terminalVelocity = 10;
    float jumpGravity;
    float jumpVelocity;

    public float groundedRaycastBoxWidth = 1;
    public float groundedRaycastDistance = 0.01f;
    bool ceiling;
    bool grounded;

    public LayerMask enemyOnlyMask = 1 << 8;
    public LayerMask ignoreRaycastMask = 1 << 2;
    LayerMask ceilingHitMask;

    public int health = 3;

    //UI
    public TextMeshProUGUI uiHealth;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        ceilingHitMask = ~(enemyOnlyMask | ignoreRaycastMask);

        CalculateJumpData();

        SetStartingUI();
    }

    void SetStartingUI()
    {
        uiHealth.text = "Health: " + health;
    }


    void Update()
    {
        CalculateJumpData();
        MovementInput();
        ActionInput();
        CheckGrounded();
        CheckCeiling();
        CalculateMovement();

        if(transform.position.y < -20)
        {
            transform.position = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
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
        if (Physics2D.BoxCast(transform.position, Vector3.one * groundedRaycastBoxWidth, 0, Vector2.down, groundedRaycastDistance)) //on ground
        {

            if (!grounded) //land on ground
            {
                if (currentCoyoteCoroutine != null)
                {
                    StopCoroutine(currentCoyoteCoroutine);
                    currentCoyoteCoroutine = null;
                }

                currentJumpCount = jumpCount;
                velocity.y = 0;

                if(currentJumpBufferCoroutine != null) // if hit ground with jump buffer
                {
                    Jump();
                }

            }
            grounded = true;
        }
        else //in air
        {
            if(grounded) //leaving ground
            {
                if (currentJumpCount > 0)
                    currentJumpCount--;

                grounded = false;

                if(jumping == false) //if leaving ground without jumping
                {
                    currentCoyoteCoroutine = StartCoroutine(CoyoteTime(coyoteJumpTime));
                }

                jumping = false;
            }
            velocity.y = Mathf.Max(velocity.y + jumpGravity * Time.deltaTime, -terminalVelocity);
        }
    }

    Coroutine currentCoyoteCoroutine;
    IEnumerator CoyoteTime(float time)
    {
        yield return new WaitForSeconds(time);
        currentCoyoteCoroutine = null;
    }

    Coroutine currentJumpBufferCoroutine;
    IEnumerator JumpBuffer(float time)
    {
        yield return new WaitForSeconds(time);
        currentJumpBufferCoroutine = null;
    }

    void CheckCeiling()
    {
        if (Physics2D.BoxCast(transform.position, Vector3.one * groundedRaycastBoxWidth, 0, Vector2.up, groundedRaycastDistance, ceilingHitMask))
        {
            if (!ceiling)
            {
                velocity.y = 0;
            }
            ceiling = true;
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

    bool jumping;
    void Jump()
    {

        if (currentJumpCount > 0 || currentCoyoteCoroutine != null)
        {
            jumping = true;

            if (!grounded)
                currentJumpCount--;

            velocity.y = jumpVelocity;
        }

        if(currentJumpCount <= 0 || !grounded)
        {
            currentJumpBufferCoroutine = StartCoroutine(JumpBuffer(jumpBufferTime));
        }

        if (currentCoyoteCoroutine != null)
        {
            StopCoroutine(currentCoyoteCoroutine);
            currentCoyoteCoroutine = null;
        }
    }


    public PolygonCollider2D PolyTrigUp;
    public PolygonCollider2D PolyTrigRight;
    public PolygonCollider2D PolyTrigDown;
    public PolygonCollider2D PolyTrigLeft;

    enum directions {UP, DOWN, RIGHT, LEFT}
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


    public void TakeDamage(int damage = 1)
    {
        if(health > 0)
        {
            health -= damage;
            uiHealth.text = "Health: " + health;
        }

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        health = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.down * groundedRaycastDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * groundedRaycastBoxWidth);
        Gizmos.DrawWireCube(transform.position + (Vector3.down * groundedRaycastDistance), Vector3.one * groundedRaycastBoxWidth);
    }


}
