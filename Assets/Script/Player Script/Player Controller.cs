using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    private float playerSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag = 1f;

    [Header("Jumping")]
    public float gravityModifier = 1f;
    public float airMultiplier = 1f;
    public float jumpForce = 10f;

    public bool onGround = false;
    public bool onSlop = false;

    [Header("Slope handling")]
    

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    private bool sprintInput;


    Vector3 moveDirection;
    
    Rigidbody rb;

    public MouvementState state;
    public enum MouvementState
    {
        walking,    
        sprinting,
        air
    }
    
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
        StateController();
        



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
        sprintInput = Input.GetKey(KeyCode.LeftShift);


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

    // Potentialy a dash code

    /* void Dash()
    {
        if (dashInput)
        {
            rb.AddForce(10f * playerSpeed * moveDirection.normalized * dashSpeed, ForceMode.Force);
        }
    }
    */
    

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

    void StateController()
    {
        // Sprinting mode 
        if (onGround && sprintInput)
        {
            state = MouvementState.sprinting;
            playerSpeed = sprintSpeed;
        }
        
        // Walking mode
        else if (onGround)
        {
            state = MouvementState.walking;
            playerSpeed = walkSpeed;
        }

        // In air mode
        else if (!onGround)
        {
            state = MouvementState.air;

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
