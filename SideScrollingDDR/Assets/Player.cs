using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    Vector2 leftDirection;
    Vector2 rightDirection;

    public float speedForce = 1;
    public float jumpForce = 1;

    Rigidbody2D rb;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }


    void Update()
    {
        leftDirection.x = Input.GetAxis("LHorizontal");
        leftDirection.y = Input.GetAxis("LVertical");

        rightDirection.x = Input.GetAxis("RHorizontal");
        rightDirection.y = Input.GetAxis("RVertical");

        rb.AddForce(leftDirection * Vector2.right * speedForce);

        if (Input.GetKeyDown(KeyCode.W))
            Jump();
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
