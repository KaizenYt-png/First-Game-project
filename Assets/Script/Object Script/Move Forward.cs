using UnityEngine;
using UnityEngine.UIElements;


public class MoveForward : MonoBehaviour
{

    private PlayerController player;
    public float speed;
    private GameManager gameManager;



    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveObjectForward();
    }

    void MoveObjectForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
        // Destroy the player game object on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            
            player.isPlayerDead = true;

        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // The obstacle get detroy when entering on collision with the wall
            Destroy(gameObject);
        }
    }


}
