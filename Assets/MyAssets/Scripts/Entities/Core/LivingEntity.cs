using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [HideInInspector] public float currentHealth;
    public bool IsDead => currentHealth <= 0;

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} received {damage} damage. Life: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}
