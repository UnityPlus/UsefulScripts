using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicROSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;

    public int numberOfObjectsToSpawn = 100;
    public float spawnDelay = 0.1f;

    public void Start()
    {
        BeginSpawning();
    }

    public void BeginSpawning()
    {
        for(int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Invoke("SpawnObject", spawnDelay * i);
        }
    }

    public void SpawnObject()
    {
        GameObject go = RecycleMaster.instance.GetFreeObject(objectToSpawn);
        go.transform.position = this.transform.position;
    }
}
