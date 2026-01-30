using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    // Call this from the UI Button OnClick() in the Inspector
    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    // Optional: async load
    public void LoadTutorialAsync()
    {
        SceneManager.LoadSceneAsync("Tutorial");
    }
}