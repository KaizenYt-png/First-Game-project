using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject carPrefabs;
    public GameObject boxPrefabs;
    private float timeToSpawnCar = 5;
    private float timeBettwenSpawnCar = 4;

    private float timeToSpawnBox = 1;
    private float timeBettwenSpawnBox = 2;

    private float spawnRangeX = 3;


    void Start()
    {
        // Spawning the object constanly
        InvokeRepeating("SpawnCar", timeToSpawnCar, timeBettwenSpawnCar);
        InvokeRepeating("SpawnBox", timeToSpawnBox, timeBettwenSpawnBox);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCar()
    {
        // Making the car spawn at random location on X axis
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(spawnPosX, 27.5f, 90f);
        Instantiate(carPrefabs, spawnPos, carPrefabs.transform.rotation);
    }

    void SpawnBox()
    {
        // Making the box spawn at random location on X axis
        float spawnPosX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(spawnPosX, 27.5f, 90f);
        Instantiate(boxPrefabs, spawnPos, boxPrefabs.transform.rotation);
    }

    
}
