// Language: C#
// This script makes a 2D enemy chase and attack the player when in range

using UnityEngine;

public class GoblinAI : MonoBehaviour
{
    // Target player
    public Transform player;

    // Movement and attack parameters
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;

    private float lastAttackTime = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Chase the player if within chase range
            if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
            {
                Vector2 direction = ((Vector2)player.position - rb.position).normalized;
                rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
            }

            // Attack the player if within attack range
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
    }

    void AttackPlayer()
    {
        // Assuming the player has the PlayerHealth component
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10); // Deal 10 damage per attack
        }
        Debug.Log("Goblin attacks the player!");
    }

    // Optional: visualize ranges in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
