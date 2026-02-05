// C# script for Unity 2D Player Attack
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 25;             // Damage dealt per attack
    public float attackRange = 1.0f;          // Range of attack
    public float attackRate = 1.0f;           // Cooldown between attacks
    public LayerMask enemyLayer;              // Layer for detecting goblins

    private float nextAttackTime = 0f;
    private Transform attackPoint;            // Position where attack is checked

    void Start()
    {
        // Create or assign attackPoint
        attackPoint = transform.Find("AttackPoint");
        if (attackPoint == null)
        {
            attackPoint = new GameObject("AttackPoint").transform;
            attackPoint.SetParent(transform);
            attackPoint.localPosition = Vector3.right; // Adjust according to player orientation
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetKeyDown(KeyCode.Space)) // Use Space key for attack
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Apply damage to each enemy in range
        foreach (Collider2D enemy in hitEnemies)
        {
            var goblinHealth = enemy.GetComponent<GoblinHealth>();
            if (goblinHealth != null)
            {
                goblinHealth.TakeDamage(attackDamage);
            }
        }

        // Optional: play attack animation
        Debug.Log("Player attacks!");
    }

    // Draw attack range in Editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}