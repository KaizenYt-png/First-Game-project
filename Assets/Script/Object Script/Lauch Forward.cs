using UnityEngine;

public class LauchForward : MonoBehaviour
{
    public float lauchForce;
    private Rigidbody boxRb;
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
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Car"))
        {
            Destroy(gameObject);
        }
    }
}
