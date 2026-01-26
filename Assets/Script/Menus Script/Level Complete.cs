using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] int nextLevelIndex = 0;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoNextLevel()
    {
        SceneManagement.LoadSceneByIndex(1);
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
