using UnityEngine;
using UnityEngine.UIElements;


public class MoveForward : MonoBehaviour
{

    private PlayerController player;
    public float speed;
    
    
    
    void Start()
    {
        
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
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Destroy the player game object on collision
        if (collision.gameObject.CompareTag("Player") && !player.hasShieldPowerUp)
        {
            Destroy(collision.gameObject);
        }

        //The obstacle get detroy when entering on collision with the wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }


}
