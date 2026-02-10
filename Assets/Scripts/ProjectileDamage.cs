using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifetime = 5f;
    public bool destroyOnImpact = true;

    void Start()
    {
        // Auto-destroy projectile after lifetime expires
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hit has the PlayerHealth component
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        // Destroy the projectile upon hitting any object
        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }

    // Use this if using 2D Physics
    private void OnTriggerEnter2D(Collider2D academics)
    {
        PlayerHealth player = academics.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}