using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Calculate desired position
        Vector3 desiredPosition = player.position + offset;

        // Apply bounds (optional)
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Lock axes if needed
        if (lockX)
            desiredPosition.x = transform.position.x;
        if (lockY)
            desiredPosition.y = transform.position.y;

        // Smoothly move camera to desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
    }
}