using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    protected float currentHealth;
    public bool IsDead => currentHealth <= 0;

    [HideInInspector] public UnityEvent<LivingEntity> OnEntityDied;

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
        OnEntityDied?.Invoke(this);
        Destroy(gameObject);
    }
}
