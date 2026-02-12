using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private bool isMoving;
    private bool isGrounded;
    private int jumpCount;
    private int maxJumps = 2;

    private float groundCheckDistance = 0.1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get input from arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        moveDirection = new Vector2(horizontal, rb.velocity.y);
        isMoving = horizontal != 0;

        // Ground check
        isGrounded = Physics2D.OverlapCircle(transform.position + Vector3.down * 0.5f, 0.15f, groundLayer);

        // Reset jump count when grounded
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            Jump();
        }

        // Update animator parameters
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VelocityY", rb.velocity.y);
        }

        // Flip sprite based on horizontal movement
        if (horizontal != 0)
        {
            transform.localScale = new Vector3(
                horizontal > 0 ? 1 : -1,
                transform.localScale.y,
                transform.localScale.z
            );
        }

        // Apply drag
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        jumpCount++;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.5f, 0.15f);
    }
}