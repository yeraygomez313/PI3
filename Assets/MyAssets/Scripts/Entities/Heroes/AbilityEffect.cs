using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class AbilityEffect
{
    public void ApplyEffectToTargets(List<LivingEntity> targets, AbilityInstance abilityInstance)
    {
        foreach (var target in targets)
        {
            if (target == null || target.IsDead)
            {
                continue; // Skip dead or null targets
            }
            ApplyEffect(target, abilityInstance);
        }
    }

    protected abstract void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance);
}

[Serializable]
public class TakeDamageEffect : AbilityEffect
{
    [SerializeField] private float damageAmount;

    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance)
    {
        target.TakeDamage(damageAmount);
    }
}

[Serializable]
public class PushEffect : AbilityEffect
{
    [SerializeField] private float pushStrength;

    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance)
    {
        target.GetComponent<LocalForceAvoidance>().ApplyPushForce(abilityInstance.transform.position, pushStrength);
    }
}

[Serializable]
public class SlowEffect : AbilityEffect
{
    [SerializeField] private float slowAmount;
    [SerializeField] private float slowDuration;
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance)
    {
        target.GetComponent<LocalForceAvoidance>().ApplySlow(slowAmount, slowDuration);
    }
}

[Serializable]
public class StunEffect : AbilityEffect
{
    [SerializeField] private float stunDuration;
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance)
    {
        target.GetComponent<LocalForceAvoidance>().ApplyStun(stunDuration);
    }
}
