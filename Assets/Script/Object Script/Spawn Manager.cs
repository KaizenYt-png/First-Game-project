using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject carPrefabs;
    private float timeToSpwan = 5;
    private float timeBettwenSpawn = 2;

    private float spawnRangeX = 3;


    void Start()
    {

        SpawnRandomObject();
        InvokeRepeating("SpawnRandomObject", timeToSpwan, timeBettwenSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRandomObject()
    {
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(spawnPosX, 27.5f, 90f);
        Instantiate(carPrefabs, spawnPos, carPrefabs.transform.rotation);
    }

    
}
