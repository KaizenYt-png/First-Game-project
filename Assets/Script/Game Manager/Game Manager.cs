using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private PlayerController player;
    public bool gameOver = false;
    public bool isGameActive = false;
    public GameObject gameOverMenu;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverMenu.SetActive(false);
        isGameActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GameOver()
    {
        
        if (player.isPlayerDead == true)
        {
            gameOverMenu.SetActive(true);
            gameOver = true;
            isGameActive = false;
        }
    }
}
