using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    public float damage = 10f;

    void OnCollisionEnter(Collision collision)
    {
        Player target = collision.gameObject.GetComponent<Player>();
        if (target != null)
        {
            // Ensure damage is float
            ApplyDamage(target, damage);
        }
    }

    void ApplyDamage(Player target, float damageAmount)
    {
        target.TakeDamage(damageAmount);
    }
}