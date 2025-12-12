using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float playerSpeed = 10f;
    public float jumpForce = 10f;
    public float gravityModifier = 1f;
    public float groundDrag = 1f;
    public float airMultiplier = 1f;

    public bool onGround = false;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;

    Vector3 moveDirection;
    
    Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the gravity
        Physics.gravity *= gravityModifier;

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
        SpeedControl();
        Jump();



        // Adding drag

        if (onGround)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void MyInput()
    {
        // Make the player move in a direction with key input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }

    private void MovePlayer()
    {
        // Making the player move in all direction 
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (onGround)
        {
            // Controll the speed on the ground
            rb.AddForce(10f * playerSpeed * moveDirection.normalized, ForceMode.Force);
        }
        else if (!onGround)
        {
            // Control the speed in the air
            rb.AddForce(10f * playerSpeed * moveDirection.normalized * airMultiplier, ForceMode.Force);
        }

        
    }

    void Jump()
    {
        // Making the player jump by pressing Space
        if (jumpInput && onGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVel.magnitude > playerSpeed)
        {
            // limit velocity if needed
            Vector3 limitedVel = flatVel.normalized * playerSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            onGround = true;
        }
        
    }
}
