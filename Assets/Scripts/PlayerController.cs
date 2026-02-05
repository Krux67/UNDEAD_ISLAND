using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    // Public variables
    public float speed = 5f; // The speed at which the player moves

    // Private variables 
    private Rigidbody2D rb; // Reference to the Rigidbody2D component attached to the player
    private Vector2 movement; // Stores the direction of player movement

    // Sprite flip handling (so player faces movement direction without rotating)
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    void Start()
    {
        // Initialize the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Prevent the player from rotating
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Cache sprite renderer (if any) to flip sprite instead of rotating it
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Determine initial facing based on spriteRenderer.flipX or localScale
        if (spriteRenderer != null)
        {
            // Assume default sprite faces right when flipX == false
            facingRight = !spriteRenderer.flipX;
        }
        else
        {
            facingRight = transform.localScale.x >= 0f;
        }
    }

    void Update()
    {
        // Only allow left/right movement (ignore vertical input)
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Set movement direction (no vertical movement)
        movement = new Vector2(horizontalInput, 0f);

        // Flip sprite to face movement direction (no rotation)
        HandleFlip(horizontalInput);
    }

    void FixedUpdate()
    {
        // Apply movement to the player in FixedUpdate for physics consistency
        rb.velocity = movement * speed;
    }

    private void HandleFlip(float horizontal)
    {
        if (horizontal > 0f && !facingRight)
        {
            Flip();
        }
        else if (horizontal < 0f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (spriteRenderer != null)
        {
            // Toggle flipX (assumes sprite faces right by default when flipX == false)
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            // Fallback: invert X scale
            Vector3 scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
        }

        facingRight = !facingRight;
    }
}