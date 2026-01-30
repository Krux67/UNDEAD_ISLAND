using UnityEngine;
using UnityEngine.SceneManagement; // Import the SceneManagement namespace
public class SceneChanger : MonoBehaviour
{
    // This function will be called when the button is clicked
    public void ChangeScene(string Tutorial)
    {
        SceneManager.LoadScene(Tutorial); // Load the scene by name
    }
}