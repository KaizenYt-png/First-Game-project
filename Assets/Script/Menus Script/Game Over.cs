using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    int sceneIndex;
    int sceneToOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        SceneManagement.LoadPreviousScene();
    }

    public void MainMenu()
    {
        SceneManagement.LoadSceneByIndex(3);
    }
}
