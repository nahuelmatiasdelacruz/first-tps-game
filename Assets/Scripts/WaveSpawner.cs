using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Tooltip("Enemy prefab to spawn")]
    public GameObject prefab;

    [Tooltip("Time between wave start and finish")]
    public float startTime, endTime;

    [Tooltip("Time between enemy spawn")]
    public float spawnRate;
    
    void Start()
    {
        InvokeRepeating("SpawnEnemy", startTime, spawnRate);
        Invoke("CancelInvoke", endTime);
    }

    void SpawnEnemy()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
