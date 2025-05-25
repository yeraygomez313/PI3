using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{
    protected float currentHealth;
    public bool IsDead => currentHealth <= 0;

    [HideInInspector] public UnityEvent<LivingEntity> OnEntityDied;

    private Tween defTween;

    private float defense;

    public bool reflectDamage;

    public virtual void TakeDamage(float damage)
    {
        if (defTween == null)
        {
            if (GetComponent<HeroController>() != null)
            {
                defense = GetComponent<HeroController>().stats.defense;
            }
            if (GetComponent<MonsterAI>() != null)
            {
                defense = GetComponent<MonsterAI>().stats.defense;
            }
        }

        currentHealth -= Mathf.Clamp(damage - ((damage * defense) / 100),10,1000);
        
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

    internal void buffDefense(float buffDuration, float buffPercent, bool reflect)
    {
        if (defTween != null && defTween.IsActive())
        {
            defTween.Kill();
        }

        if (reflect)
        {
            reflectDamage = true;
        }

        defense = GetComponent<HeroController>().stats.defense + ((GetComponent<HeroController>().stats.defense * buffPercent) / 100);

        defTween = DOVirtual.DelayedCall(buffDuration, () =>
        {
            if (defTween == null || defTween.IsComplete())
            {
                defense = GetComponent<HeroController>().stats.defense;
            }
        }, false);
    }
}
