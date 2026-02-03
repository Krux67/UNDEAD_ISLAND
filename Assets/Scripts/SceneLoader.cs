
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadTutorial()
    {
        Debug.Log("Button clicked!");
        SceneManager.LoadScene("Tutorial");
    }
}