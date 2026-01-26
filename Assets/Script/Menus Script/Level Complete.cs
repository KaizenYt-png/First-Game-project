using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] int nextLevelIndex = 1;
    

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

    public void GoNextLevel()
    {
        SceneManagement.LoadSceneByIndex(nextLevelIndex);
    }

    public void RestartLevel()
    {
        SceneManagement.LoadPreviousScene();
    }

    public void MainMenu()
    {
        SceneManagement.LoadSceneByIndex(3);
    }
}
