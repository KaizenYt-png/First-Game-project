using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    private PlayerController player;
    public float speedBoostAdd = 5;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    void HasSpeedPowerUp()
    {
        if (player.hasSpeedPowerUp == true)
        {
            // Add speed value to the walk speed and sprint speed
            player.sprintSpeed = player.sprintSpeed + speedBoostAdd;
            player.walkSpeed = player.walkSpeed + speedBoostAdd;
            Debug.Log("SpeedBoost applied");
        }
    }

    void HasShieldPowerUp()
    {
        if(player.hasSpeedPowerUp == true)
        {

        }
    }
}
