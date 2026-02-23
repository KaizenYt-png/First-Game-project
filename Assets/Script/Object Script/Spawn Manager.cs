using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] carPrefabs;
    public GameObject[] objectPrefabs;
    private EndSpawning endSpawning;

    public bool spawnCar;
    public bool spawnObject;

    public float carSpawnRate = 0.5f;
    public float objectSpawnRate = 0.5f;
    /*public float timeToSpawnBox = 1;
      public float timeBettwenSpawnBox = 2; */

    public float spawnRangeX1 = 3;
    public float spawnRangeX2 = 3;
    public float spawnPosY = 27.5f;
    public float spawnPosZ = 90f;

    public bool gameActive = true;

    


    void Start()
    {
        endSpawning = GameObject.Find("End Spawning Trigger").GetComponent<EndSpawning>();

        gameActive = true;
        StartCoroutine(SpawnCar());
        StartCoroutine(SpawnObject());


    }

    // Update is called once per frame
    void Update()
    {
         
    }




    public IEnumerator SpawnCar()
    {
        if (spawnCar == true)
        {
            while (endSpawning.gameActive)
            {
                yield return new WaitForSeconds(carSpawnRate);
                // Making the car spawn at random location on X axis
                int carIndex = Random.Range(0, carPrefabs.Length);
                float spawnPosX = Random.Range(spawnRangeX1, spawnRangeX2);
                Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
                Instantiate(carPrefabs[carIndex], spawnPos, carPrefabs[carIndex].transform.rotation);
            }
        }
    }

    public IEnumerator SpawnObject()
    {
        if (spawnObject == true)
        {
            while (endSpawning.gameActive)
            {
                yield return new WaitForSeconds(objectSpawnRate);
                // Making the box spawn at random location on X axis
                int objectIndex = Random.Range(0, objectPrefabs.Length);
                float spawnPosX = Random.Range(spawnRangeX1, spawnRangeX2);
                Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
                Instantiate(objectPrefabs[objectIndex], spawnPos, objectPrefabs[objectIndex].transform.rotation);
            }
        }
        
        
    }

}
