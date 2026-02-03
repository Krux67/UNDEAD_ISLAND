// Language: C#
// This script manages player health, taking damage, and game over conditions.
using UnityEngine;
using UnityEngine.SceneManagement; // Needed if you want to reload the scene

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum player health
    private int currentHealth;

    public HealthBar healthBar; // Optional: reference to UI Health bar

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // Method to deal damage to the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Player death logic
    void Die()
    {
        Debug.Log("Player has died!");
        // Reload current scene or implement game over UI
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Optional: for healing player
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }
}