using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Load scene at the provided index - current scene index gets saved
    public static void LoadSceneByIndex(int sceneIndex)
    {
        PlayerPrefs.SetInt("PreviousSceneIndex", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("Loading scene " + sceneIndex);
    }

    // Go back one scene using saved index
    public static void LoadPreviousScene()
    {
        if (!PlayerPrefs.HasKey("PreviousSceneIndex"))
            Debug.LogError("There is no previous scene!");
        else
            SceneManager.LoadScene(PlayerPrefs.GetInt("PreviousSceneIndex"));

        Debug.Log("Loading scene " + PlayerPrefs.HasKey("PreviousSceneIndex"));
    }
}
