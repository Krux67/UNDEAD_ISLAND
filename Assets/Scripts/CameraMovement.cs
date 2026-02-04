using UnityEngine;

/// <summary>
/// 2D camera that snaps to the Player position (no smoothing) so it won't drift or move by itself.
/// Attach this to the Main Camera. If no target is assigned the script will try to find
/// a PlayerController2D or a GameObject tagged "Player".
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [Tooltip("Transform the camera will follow. If null, script will try to auto-find the player.")]
    [SerializeField] private Transform target;

    [Tooltip("World offset from the target. Z should usually be -10 for 2D cameras.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Optional bounds")]
    [Tooltip("Enable to clamp camera position inside min/max bounds.")]
    [SerializeField] private bool useBounds = false;
    [Tooltip("Minimum world position (bottom-left) the camera center may reach.")]
    [SerializeField] private Vector2 minBounds = Vector2.zero;
    [Tooltip("Maximum world position (top-right) the camera center may reach.")]
    [SerializeField] private Vector2 maxBounds = Vector2.zero;

    void Start()
    {
        // Auto-assign target if not set in inspector
        if (target == null)
        {
            var pc = FindObjectOfType<PlayerController2D>();
            if (pc != null)
            {
                target = pc.transform;
            }
            else
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) target = playerObj.transform;
            }
        }

        // Ensure camera uses correct Z on start
        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
    }

    // Use LateUpdate so camera follows after player movement in Update/FixedUpdate
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = offset.z; // ensure correct camera Z

        if (useBounds)
        {
            Camera cam = Camera.main;
            if (cam != null && cam.orthographic)
            {
                float vertExtent = cam.orthographicSize;
                float horzExtent = vertExtent * (Screen.width / (float)Screen.height);

                float minX = minBounds.x + horzExtent;
                float maxX = maxBounds.x - horzExtent;
                float minY = minBounds.y + vertExtent;
                float maxY = maxBounds.y - vertExtent;

                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            }
            else
            {
                // If no orthographic camera found, clamp without extents
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }
        }

        // Snap the camera directly to the target position (no smoothing)
        transform.position = desiredPosition;
    }
}