using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] carPrefabs;
    public GameObject boxPrefabs;
    public GameManager gameManager;
    
    public float timeToSpawnCar = 5;
    public float timeBettwenSpawnCar = 4;

    public float timeToSpawnBox = 1;
    public float timeBettwenSpawnBox = 2;

    public float spawnRangeX1 = 3;
    public float spawnRangeX2 = 3;
    public float spawnPosY = 27.5f;
    public float spawnPosZ = 90f;


    void Start()
    {
        while (gameManager.isGameActive == true)
        {
            Debug.Log("Spawing object");
            InvokeRepeating("SpawnCar", timeToSpawnCar, timeBettwenSpawnCar);
            InvokeRepeating("SpawnBox", timeToSpawnBox, timeBettwenSpawnBox);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCar()
    {
        // Making the car spawn at random location on X axis
        int carIndex = Random.Range(0,carPrefabs.Length);
        float spawnPosX = Random.Range(spawnRangeX1, spawnRangeX2);
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        Instantiate(carPrefabs[carIndex], spawnPos, carPrefabs[carIndex].transform.rotation);
    }

    void SpawnBox()
    {
        // Making the box spawn at random location on X axis
        float spawnPosX = Random.Range(spawnRangeX1, spawnRangeX2);
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        Instantiate(boxPrefabs, spawnPos, boxPrefabs.transform.rotation);
    }

    
}
