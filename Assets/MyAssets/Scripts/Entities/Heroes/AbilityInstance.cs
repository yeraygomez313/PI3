using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AbilityInstance : MonoBehaviour
{
    [field:SerializeField] public HabilityStats AbilityStats { get; private set; }
    private float timeSinceCreation = 0;
    private float timeOfLastTick = 0;
    private Collider2D hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<Collider2D>();
        hitCollider.isTrigger = true;
    }

    public void Initialize(Transform origin, Quaternion orientation, bool setOriginAsParent = false)
    {
        if (setOriginAsParent)
        {
            transform.SetParent(origin);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.position = origin.position;
        }

        SearchForTargetsAndApplyEffects(AbilityStats);
    }

    private void SearchForTargetsAndApplyEffects(HabilityStats abilityStats)
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

        foreach (var effect in abilityStats.abilityEffects)
        {
            effect.ApplyEffectToTargets(targets, this);
        }
    }

    private void Update()
    {
        timeSinceCreation += Time.deltaTime;

        if (timeSinceCreation >= AbilityStats.duration)
        {
            Destroy(gameObject);
            return;
        }

        if (timeSinceCreation - timeOfLastTick > AbilityStats.tickDamage)
        {
            timeOfLastTick = timeSinceCreation;
            SearchForTargetsAndApplyEffects(AbilityStats);
        }
    }
}
