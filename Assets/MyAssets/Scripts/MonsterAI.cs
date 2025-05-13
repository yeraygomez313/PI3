using UnityEngine;

public class MonsterAI : LivingEntity
{
    public MonsterStats stats;

    private Transform heroTarget;
    private HeroController heroController;
    private float attackCooldown;

    private void Start()
    {
        currentHealth = stats.maxHealth;
        GameObject heroObj = GameObject.FindGameObjectWithTag("Hero");

        if (heroObj != null)
        {
            heroTarget = heroObj.transform;
            heroController = heroObj.GetComponent<HeroController>();
        }
    }

    private void Update()
    {
        if (IsDead || heroController == null || heroController.IsDead) return;

        float distance = Vector2.Distance(transform.position, heroTarget.position);

        if (distance <= stats.atRange)
        {
            TryAttackHero();
        }
        else
        {
            MoveTowardsHero();
        }

        attackCooldown -= Time.deltaTime;
    }

    private void MoveTowardsHero()
    {
        Vector2 direction = (heroTarget.position - transform.position).normalized;
        transform.position += (Vector3)(direction * stats.mvSpeed * Time.deltaTime);
    }

    private void TryAttackHero()
    {
        if (attackCooldown <= 0f)
        {
            float finalDamage = Mathf.Max(1, stats.attack - heroController.stats.defense);
            heroController.TakeDamage(finalDamage);
            attackCooldown = 1f / stats.atSpeed;
        }
    }
}
