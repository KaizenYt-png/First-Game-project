using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Sensitivity of the camera
    [SerializeField]
    public float sensX;
    public float sensY;

    public Transform orientation;

    public float xRotation;
    public float yRotation;
    
    void Start()
    {
        // Make the cursor lock in the center of the screen (invisible too)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Make the camera move on the X and Y axis with mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // I think this is the equation for making the camera move around
        yRotation += mouseX;
        xRotation -= mouseY;

        // Prevent the player from looking more then 90 degree up and down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Change the player orientation by the direction he is looking at
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
