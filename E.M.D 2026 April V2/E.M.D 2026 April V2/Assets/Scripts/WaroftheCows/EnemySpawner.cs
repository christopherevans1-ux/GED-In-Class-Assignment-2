using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject cowPrefab;
    public Transform cowSpawnTransform;
    public float timeSinceSpawn = 0;
    public float spawnTimer = 3;


    void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn >= spawnTimer)
        {
            GameObject cow = Instantiate(cowPrefab, cowSpawnTransform.position, Quaternion.identity);
            timeSinceSpawn = 0;
        }
    }
}
