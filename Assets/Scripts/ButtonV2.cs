using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach this to a UI Button. When clicked it will load the "Tutorial" scene.
/// Make sure "Tutorial" is added to __File > Build Settings...__.
/// </summary>
[RequireComponent(typeof(Button))]
public class TutorialButton : MonoBehaviour
{
    [Tooltip("Scene name to load (ensure this scene is listed in Build Settings)")]
    public string sceneName = "Tutorial";

    [Tooltip("If true, use build index instead of sceneName")]
    public bool useBuildIndex = false;
    [Tooltip("Build index to load when useBuildIndex is true")]
    public int buildIndex = 0;

    [Tooltip("Auto-wire this Button's OnClick to call OnClickLoadScene")]
    public bool autoHookButton = true;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.LogError("TutorialButton requires a Button component.");
    }

    void Start()
    {
        if (autoHookButton && button != null)
            button.onClick.AddListener(OnClickLoadScene);
    }

    /// <summary>
    /// Call this from the Button OnClick (or let the script auto-hook it).
    /// Loads the configured scene by name or build index.
    /// </summary>
    public void OnClickLoadScene()
    {
        if (useBuildIndex)
        {
            if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"TutorialButton: buildIndex {buildIndex} is out of range.");
                return;
            }
            SceneManager.LoadScene(buildIndex);
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("TutorialButton: sceneName is empty. Set a scene name or enable useBuildIndex.");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"TutorialButton: scene '{sceneName}' is not listed in Build Settings. Add it via __File > Build Settings...__.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private bool IsSceneInBuildSettings(string name)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string n = System.IO.Path.GetFileNameWithoutExtension(path);
            if (n == name) return true;
        }
        return false;
    }
}