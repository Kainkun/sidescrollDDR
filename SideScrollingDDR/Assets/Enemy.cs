using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health = 1;
    public float moveSpeed = 1;

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

        Vector2 nextPosition = player.transform.position;
        if (Mathf.Abs((player.transform.position - transform.position).x) > Mathf.Abs((player.transform.position - transform.position).y))
        {
            print("x>y");
            nextPosition.y = 0;
        }
        else
        {
            print("y>x");
            nextPosition.x = 0;
        }
        rb.MovePosition(Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * moveSpeed));
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
}
