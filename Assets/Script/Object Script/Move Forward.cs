using UnityEngine;
using UnityEngine.UIElements;


public class MoveForward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        //The obstacle get detroy when entering on collision with the wall
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Destroy the player game object on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
    }


}
