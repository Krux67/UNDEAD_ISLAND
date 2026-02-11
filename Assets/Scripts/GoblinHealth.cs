using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 50;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Goblin Health: " + health);
        if (health <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Goblin died!");
        Destroy(gameObject); // Remove goblin from scene
    }
}