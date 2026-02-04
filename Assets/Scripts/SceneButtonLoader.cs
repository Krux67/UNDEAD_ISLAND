using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonLoader : MonoBehaviour
{
    // Put exactly: Tutorial
    [SerializeField] private string sceneName = "Tutorial";

    // Hook this up to your UI Button's OnClick()
    public void LoadScene()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is empty. Set it in the Inspector.");
            return;
        }

        // Optional safety check (only works if the scene is in Build Settings)
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' can't be loaded. Is it added to Build Settings and spelled correctly?");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
