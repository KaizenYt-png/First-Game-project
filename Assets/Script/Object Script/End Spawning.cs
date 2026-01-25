using UnityEngine;

public class EndSpawning : MonoBehaviour
{
    public bool gameActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameActive = false;
        }
    }
}
