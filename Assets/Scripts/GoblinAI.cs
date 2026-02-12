using UnityEngine;

public class GoblinAI : MonoBehaviour
{
    [Header("Detection & Combat")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f; public int damage = 10;
    public float attackRate = 1f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public Transform player;

    private float nextAttackTime;
    private bool facingRight = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            Chase();
        }
    }

    void Chase()
    {
        // Move towards player on X axis only (Platformer style)
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Flip to face player
        if (player.position.x > transform.position.x && !facingRight) Flip();
        else if (player.position.x < transform.position.x && facingRight) Flip();
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // Try to find the PlayerHealth script on the target
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Goblin Attacks!");
            }

            nextAttackTime = Time.time + attackRate;
        }
    }

    void Flip()
    {
        facingRight = !facingRight; Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Visualizes detection and attack ranges in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}