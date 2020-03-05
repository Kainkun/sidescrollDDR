using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public bool loop;
    bool looping;
    public float[] spawnTiming;

    public GameObject spawnObject;
    public Transform spawnPosition;

    public float spawnRadius;

    void Spawn()
    {
        Instantiate(spawnObject, (Vector2)spawnPosition.position + (Random.insideUnitCircle * spawnRadius), Quaternion.identity);
    }

    Coroutine currentLoop;
    IEnumerator SpawnTimed()
    {
        for (int i = 0; i < spawnTiming.Length; i++)
        {
            Spawn();
            yield return new WaitForSeconds(spawnTiming[i]);
        }

        if(looping)
        {
            currentLoop = StartCoroutine(SpawnTimed());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (loop)
                looping = true;

            if(spawnTiming.Length == 0)
                Spawn();
            else
                currentLoop = StartCoroutine(SpawnTimed());

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (loop)
        {
            looping = false;

            if(currentLoop != null)
            {
                StopCoroutine(currentLoop);
                currentLoop = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
        Gizmos.DrawWireSphere(spawnPosition.position, spawnRadius);
    }


}
