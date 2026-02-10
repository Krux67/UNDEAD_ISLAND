// Language: C#
using UnityEngine;

public class Teleporter2D : MonoBehaviour
{
    // Assign the destination in the Inspector
    public Transform teleportDestination;

    // Optional: cooldown to avoid instant re-teleporting
    public float teleportCooldown = 0.5f;
    private bool canTeleport = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canTeleport)
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private System.Collections.IEnumerator TeleportPlayer(GameObject player)
    {
        canTeleport = false;

        // Move the player instantly to the destination
        player.transform.position = teleportDestination.position;

        // Optional: You can add visual or sound effects here
        // Example: Instantiate(teleportEffectPrefab, player.transform.position, Quaternion.identity);

        // Cooldown to prevent re-triggering immediately
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
}