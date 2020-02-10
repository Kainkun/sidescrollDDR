using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMng : MonoBehaviour
{
    public float interval = 0.33f;
    public GameObject enemy;
    bool spawning;
    Player player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        player = Player.instance;
        spawning = true;
        StartCoroutine(SpawnLoop());
    }


    void Update()
    {
        
    }

    IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(interval);

            Vector2 spawnPos = GetSpawnPosition();
            if(spawnPos != Vector2.zero)
                Instantiate(enemy, (Vector2)player.transform.position + spawnPos, Quaternion.identity);
        }
        yield return null;  
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        RaycastHit2D ray = Physics2D.Raycast(player.transform.position, spawnDirection, 10);
        Debug.DrawRay(player.transform.position, spawnDirection * 10);

        if (ray.collider == null)
            return spawnDirection * 10;
        else
        {
            if (ray.distance < 2)
                return Vector2.zero;
            else
                return spawnDirection * (ray.distance - 2);
        }


    }

}
