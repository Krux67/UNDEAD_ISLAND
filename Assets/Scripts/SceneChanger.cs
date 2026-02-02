using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Set this to EXACTLY the scene name in your Project (case-sensitive).")]
    public string sceneToLoad = "tutorial";

    public void LoadScene()
    {
        Debug.Log($"[SceneChanger] Attempting to load scene: '{sceneToLoad}'");

        // Check if scene is in Build Settings
        if (!IsSceneInBuild(sceneToLoad))
        {
            Debug.LogError($"[SceneChanger] Scene '{sceneToLoad}' is NOT in Build Settings. " +
                           $"Go to File > Build Settings and add it.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
                return true;
        }

        return false;
    }
}
