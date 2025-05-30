using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public abstract class AbilityEffect
{
    [SerializeField] private bool affectsHeroes = false;

    public void ApplyEffectToTargets(List<LivingEntity> targets, AbilityInstance abilityInstance, float heroAttack)
    {
        foreach (var target in targets)
        {
            bool applyEffect;

            if (target is HeroController)
            {
                applyEffect = affectsHeroes ? true : false;
            }
            else if (target is MonsterAI)
            {
                applyEffect = !affectsHeroes ? true : false;
            }
            else
            {
                applyEffect = false; // Default to not applying effect for other types
                Debug.LogWarning($"Target {target.name} is not a Hero or Monster. Effect will not be applied.");
            }

            if (target == null || target.IsDead || !applyEffect)
            {
                continue; // Skip dead or null targets
            }

            ApplyEffect(target, abilityInstance, heroAttack);
        }
    }

    public void ApplyEffectToTarget(LivingEntity target, AbilityInstance abilityInstance, float heroAttack)
    {
        bool applyEffect;

        if (target is HeroController)
        {
            applyEffect = affectsHeroes ? true : false;
        }
        else if (target is MonsterAI)
        {
            applyEffect = !affectsHeroes ? true : false;
        }
        else
        {
            applyEffect = false; // Default to not applying effect for other types
            Debug.LogWarning($"Target {target.name} is not a Hero or Monster. Effect will not be applied.");
        }

        if (target == null || target.IsDead || !applyEffect)
        {
            return; // Skip dead or null targets
        }

        ApplyEffect(target, abilityInstance, heroAttack);
    }

    protected abstract void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack);
}

[Serializable]
public class DealDamageEffect : AbilityEffect
{
    [SerializeField] private float damageMultiplier;

    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        target.TakeDamage(attack*damageMultiplier);
    }
}

[Serializable]
public class PushEffect : AbilityEffect
{
    [SerializeField] private float pushStrength;

    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        target.GetComponent<LocalForceAvoidance>().ApplyPushForce(abilityInstance.transform.position, pushStrength);
    }
}

[Serializable]
public class SlowEffect : AbilityEffect
{
    [SerializeField] private float slowAmount;
    [SerializeField] private float slowDuration;
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        target.GetComponent<LocalForceAvoidance>().ApplySlow(slowAmount, slowDuration);
    }
}

[Serializable]
public class StunEffect : AbilityEffect
{
    [SerializeField] private float stunDuration;
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        target.GetComponent<LocalForceAvoidance>().ApplyStun(stunDuration);
    }
}

[Serializable]
public class BuffEffect : AbilityEffect
{
    [SerializeField] private float buffDuration;
    [SerializeField] private float buffPercent;
    [SerializeField] private bool reflectDamage;
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        target.buffDefense(buffDuration, buffPercent, reflectDamage);
    }
}

[Serializable]
public class SpawnEffect : AbilityEffect
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnAngle = 90f;
    [SerializeField] private bool spawnRandomly = false;
    [SerializeField] private float spawnDelay = 0.25f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private bool linkedToTarget;
    
    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance, float attack)
    {
        MonoBehaviour spawner = target.GetComponent<MonoBehaviour>();
        spawner.StartCoroutine(SpawnWithDelay(target.transform, abilityInstance, attack));
    }

    private IEnumerator SpawnWithDelay(UnityEngine.Transform origin, AbilityInstance abilityInstance, float attack)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = origin.position;
            float halfAngle = spawnAngle / 2f;

            Quaternion spawnRotation;

            if (spawnRandomly)
            {
                float randomAngle = UnityEngine.Random.Range(-halfAngle, halfAngle);
                spawnRotation = Quaternion.Euler(0, 0, randomAngle) * origin.rotation;
            }
            else
            {
                float angleOffset = i * (spawnAngle / (spawnCount - 1)) - halfAngle;
                spawnRotation = Quaternion.Euler(0, 0, angleOffset) * origin.rotation;
            }

            GameObject spawnedObject = UnityEngine.Object.Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

            if (prefabToSpawn.GetComponent<Projectile>() != null)
            {
                Vector2 direction = abilityInstance.transform.rotation * Vector2.right;
                spawnedObject.GetComponent<Projectile>().Initialize(direction, attack, false);
            }

            if (linkedToTarget)
            {
                spawnedObject.transform.SetParent(origin);
                spawnedObject.transform.localPosition = Vector3.zero;
            }

            UnityEngine.Object.Destroy(spawnedObject, lifetime);
            //Ability spawnedAbility = spawnedObject.GetComponent<Ability>();

            //if (spawnedAbility != null)
            //{
            //    spawnedAbility.ActivateAbility(origin, spawnRotation, setOriginAsParent);
            //}
            //else
            //{
            //    Debug.LogWarning($"Spawned object {spawnedObject.name} does not have an IAbility component.");
            //}

            yield return wait;
        }
    }
}
