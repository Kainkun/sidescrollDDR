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
            Vector2 spawnPos = Random.insideUnitCircle.normalized * Random.Range(15f,20f);
            Instantiate(enemy, (Vector2)player.transform.position + spawnPos, Quaternion.identity);
        }
        yield return null;  
    }

}
