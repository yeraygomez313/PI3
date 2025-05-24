using System.Collections.Generic;
using UnityEngine;

public class AbilityInstance : MonoBehaviour
{
    public AbilityData AbilityData { get; protected set; }

    private float timeSinceCreation = 0;
    private float timeOfLastTick = 0;
    private Collider2D hitCollider;

    public void Initialize(AbilityData abilityData, LivingEntity caster, Quaternion orientation, float casterAttack)
    {
        AbilityData = abilityData;

        if (AbilityData.LinkedToCaster)
        {
            transform.SetParent(caster.transform);
            transform.localPosition = Vector3.zero;
        }

        hitCollider = Instantiate(AbilityData.AreaPrefab, transform.position, orientation, transform).GetComponent<Collider2D>();
        hitCollider.transform.localPosition += transform.forward * AbilityData.ForwardOffset;
        if (hitCollider.GetComponentsInChildren<Collider2D>() != null)
        {
            foreach (Collider2D collider in hitCollider.GetComponentsInChildren<Collider2D>())
            {
                collider.transform.localPosition += transform.forward * AbilityData.ForwardOffset;
            }
        }
        hitCollider.isTrigger = true;
        hitCollider.transform.localScale = Vector3.one * AbilityData.Scale;

        foreach (var effect in AbilityData.OnAwakeEffects)
        {
            effect.ApplyEffectToTarget(caster, this, casterAttack);
        }

        foreach (var effect in AbilityData.OnEntityContactEffects)
        {
            effect.ApplyEffectToTargets(SearchForTargets(), this, casterAttack);
        }
    }

    private List<LivingEntity> SearchForTargets()
    {
        List<LivingEntity> targets = new List<LivingEntity>();

        if (hitCollider is CircleCollider2D circleCollider)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
            foreach (var col in colliders)
            {
                LivingEntity entity = col.GetComponent<LivingEntity>();
                if (entity != null && !entity.IsDead)
                {
                    targets.Add(entity);
                }
            }
        }
        else if (hitCollider is BoxCollider2D boxCollider)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
            foreach (var col in colliders)
            {
                LivingEntity entity = col.GetComponent<LivingEntity>();
                if (entity != null && !entity.IsDead)
                {
                    targets.Add(entity);
                }
            }
        }

        return targets;
    }

    private void Update()
    {
        timeSinceCreation += Time.deltaTime;

        if (timeSinceCreation >= AbilityData.Duration)
        {
            Destroy(gameObject);
            return;
        }

        if (timeSinceCreation - timeOfLastTick > AbilityData.TickRate)
        {
            timeOfLastTick = timeSinceCreation;
            foreach (var effect in AbilityData.OnAwakeEffects)
            {
                effect.ApplyEffectToTargets(SearchForTargets(), this, GetComponentInParent<HeroStats>().attack);
            }
        }
    }

    public void SpawnObject(GameObject go, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject spawnedObject;

        if (parent != null)
        {
            spawnedObject = Instantiate(go, position, rotation, parent);
        }
        else
        {
            spawnedObject = Instantiate(go, position, rotation);
        }

        spawnedObject.transform.localScale = Vector3.one * AbilityData.Scale;
        spawnedObject.transform.SetParent(transform);
    }
}
