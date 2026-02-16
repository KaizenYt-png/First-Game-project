using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class MoveForward : MonoBehaviour
{

    private PlayerController player;
    public float speed;
    public float lauchForce;
    private GameManager gameManager;
    public ParticleSystem explosionParticle;
    private AudioSource audioSource;
    public AudioClip explosionSF;
    private Rigidbody carRb;




    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
        carRb = GetComponent<Rigidbody>();
        carRb.AddForce(Vector3.back * lauchForce, ForceMode.Impulse);
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
        // Desactivate the player game object on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManagement.LoadSceneByIndex(3);

        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // The obstacle get detroy when entering on collision with the wall
            AudioSource.PlayClipAtPoint(explosionSF, transform.position);
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            
            
        }
    }


}
