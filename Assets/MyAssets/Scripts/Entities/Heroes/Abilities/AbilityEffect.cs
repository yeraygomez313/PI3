using System;
using System.Collections;
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

    public void ApplyEffectToTarget(LivingEntity target, AbilityInstance abilityInstance)
    {
        if (target == null || target.IsDead)
        {
            return; // Skip dead or null targets
        }
        ApplyEffect(target, abilityInstance);
    }

    protected abstract void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance);
}

[Serializable]
public class DealDamageEffect : AbilityEffect
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

[Serializable]
public class SpawnEffect : AbilityEffect
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnAngle = 90f;
    [SerializeField] private bool spawnRandomly = false;
    [SerializeField] private float spawnDelay = 0.25f;
    [SerializeField] private bool linkedToTarget;

    protected override void ApplyEffect(LivingEntity target, AbilityInstance abilityInstance)
    {
        MonoBehaviour spawner = target.GetComponent<MonoBehaviour>();
        spawner.StartCoroutine(SpawnWithDelay(target.transform));
    }

    private IEnumerator SpawnWithDelay(Transform origin)
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

            if (linkedToTarget)
            {
                spawnedObject.transform.SetParent(origin);
                spawnedObject.transform.localPosition = Vector3.zero;
            }

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
