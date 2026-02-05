// Language: C#
// Stationary ranged enemy AI
using UnityEngine;

public class StationaryRangedEnemy : MonoBehaviour
{
    public Transform player;          // Player reference
    public float attackRange = 5f;    // Maximum distance for attacking
    public float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime = 0f;

    public GameObject projectilePrefab;   // Projectile to shoot
    public Transform shootPoint;          // Where the projectile spawns from
    public float projectileSpeed = 5f;    // Speed of the projectile

    public Animator animator; // Optional, for animation control

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else Debug.LogError("Player not found! Assign the player in the inspector.");
        }
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // Aim or rotate toward the player (optional)
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            AttackPlayer();
        }
        else
        {
            // Outside attack range, remain stationary
            if (animator != null) animator.SetBool("isMoving", false);
        }
    }

    void AttackPlayer()
    {
        if (Time.time > lastAttackTime + attackCooldown)
        {
            if (projectilePrefab != null && shootPoint != null)
            {
                GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = shootPoint.right * projectileSpeed;
                }
            }

            if (animator != null) animator.SetTrigger("attack");

            lastAttackTime = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
