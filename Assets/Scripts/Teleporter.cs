using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure your Player object has the "Player" tag in the Inspector
        if (collision.CompareTag("Player"))
        {
            // Move the Player to the destination
            collision.transform.position = destination.position;

            // Move the Camera instantly to the destination
            if (Camera.main != null)
            {
                Vector3 newCamPos = destination.position;
                newCamPos.z = -10f; // Keep the camera at the correct 2D depth
                Camera.main.transform.position = newCamPos;
            }
        }
    }
}