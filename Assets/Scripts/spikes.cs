using UnityEngine;

public class KillBrick : MonoBehaviour
{
    // Reference to the player (optional - you can also use tags)
    public GameObject player;

    // Alternative: use a tag to identify the player
    private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that touched this brick is the player
        if (collision.CompareTag(playerTag))
        {
            KillPlayer(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Alternative method if using regular colliders instead of triggers
        if (collision.gameObject.CompareTag(playerTag))
        {
            KillPlayer(collision.gameObject);
        }
    }

    private void KillPlayer(GameObject playerObject)
    {
        // Option 1: Destroy the player
        Destroy(playerObject);

        // Option 2: Disable the player
        // playerObject.SetActive(false);

        // Option 3: Trigger a death animation or event
        // PlayerManager.Instance.Die();

        // Option 4: Reload the scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
