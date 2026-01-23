using UnityEngine;
using UnityEngine.SceneManagement;

public class LauchForward : MonoBehaviour
{
    public float lauchForce;
    private Rigidbody boxRb;
    private PlayerController player;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        boxRb = GetComponent<Rigidbody>();
        boxRb.AddForce(Vector3.back * lauchForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shield"))
        {
            // If the player has shield destroy any object that collide with it
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Desactivate the player game object on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            
            SceneManager.LoadSceneAsync("Game Over");

        }
        else if (
                 collision.gameObject.CompareTag("Car") || 
                 collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            
        }
    }
}
