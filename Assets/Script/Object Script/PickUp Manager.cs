using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    private PlayerController player;
    public float walkingSpeedBoostSpeed;
    public float sprintingSpeedBoostSpeed;
    private float walkingSpeed;
    private float sprintingSpeed;
    public int speedPowerUpCountDown = 3;
    public int shieldPowerUpCountDown = 5;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();

        walkingSpeed = player.walkSpeed;
        sprintingSpeed = player.sprintSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
        HasSpeedPowerUp();
        HasShieldPowerUp();
    }

    private void FixedUpdate()
    {
        
    }

    void HasSpeedPowerUp()
    {
        if (player.hasSpeedPowerUp == true)
        {
            // Making the normal speed of the player the speed of the speed boost
            player.sprintSpeed = sprintingSpeedBoostSpeed;
            player.walkSpeed = walkingSpeedBoostSpeed;
            // Adding a countdown
            StartCoroutine(SpeedUpCountDown(speedPowerUpCountDown));
            Debug.Log("SpeedBoost applied");
        }
    }

    void HasShieldPowerUp()
    {
        if(player.hasShieldPowerUp == true)
        {

            // Adding a countdown
            StartCoroutine(ShieldUpCountDown(shieldPowerUpCountDown));
            Debug.Log("Shield applied");
        }
    }
    IEnumerator ShieldUpCountDown(int seconde)
    {
        yield return new WaitForSeconds(seconde);
        player.hasShieldPowerUp = false; 
    }

    IEnumerator SpeedUpCountDown(int seconde)
    {
        yield return new WaitForSeconds(seconde);

        // Make the player go back to initial speed
        player.walkSpeed = walkingSpeed;
        player.sprintSpeed = sprintingSpeed;

        player.hasSpeedPowerUp = false;
    }
}
