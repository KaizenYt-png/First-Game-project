using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel1()
    {
        SceneManagement.LoadSceneByIndex(0);
    }
    public void LoadLevel2()
    {
        SceneManagement.LoadSceneByIndex(1);
    }
}
