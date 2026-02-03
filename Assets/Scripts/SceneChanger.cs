using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Call this from a UI Button
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
