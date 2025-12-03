using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        // Make the camera holder go in the camera position in the player object 
        transform.position = cameraPosition.position;
    }
}
