using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float playerSpeed = 10f;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;
    

    Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Call the rigid body and make is rotation freeze
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // Call the MovePlayer Methode
        MovePlayer();
    }

    
    void Update()
    {
        MyInput();  
    }

    void MyInput()
    {
        // Make the player move in a direction with key input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Making the player move in all direction 
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * playerSpeed * 10f, ForceMode.Force );
    }
}
