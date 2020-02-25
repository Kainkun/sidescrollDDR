using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health = 1;
    public float moveSpeed = 1;
    public float centeringSpeed = 3;

    Player player;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = Player.instance;
    }


    void Update()
    {
        if(Vector2.Distance(player.transform.position, transform.position) < 15)
            Movement();
    }

    void Movement()
    {
        Vector2 nextVector = player.transform.position - transform.position;

        nextVector = nextVector.normalized;
        if (Mathf.Abs((player.transform.position - transform.position).x) > Mathf.Abs((player.transform.position - transform.position).y))
        {
            nextVector.y *= centeringSpeed;
        }
        else
        {
            nextVector.x *= centeringSpeed;
        }

        rb.MovePosition((Vector2)transform.position + (nextVector * Time.deltaTime * moveSpeed));
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
