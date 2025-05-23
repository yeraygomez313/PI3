using UnityEngine;

public class HeroController : LivingEntity
{
    public HeroStats stats;
    public LayerMask enemyLayer;

    private float attackCooldown = 0f;

    private void Start()
    {
        currentHealth = stats.maxHealth;
    }

    private void Update()
    {
        if (IsDead) return;

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            AutoAttackEnemies();
            attackCooldown = 1f / stats.atSpeed;
        }
    }

    private void AutoAttackEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, stats.atRange, enemyLayer);
        Debug.Log("Autoattack: detected " + enemies.Length + " enemies");

        foreach (var enemy in enemies)
        {
            MonsterAI monster = enemy.GetComponent<MonsterAI>();
            if (monster != null && !monster.IsDead)
            {
                float finalDamage = Mathf.Max(1, stats.attack /*- monster.stats.defense*/);
                monster.TakeDamage(finalDamage);
                Debug.Log($"{gameObject.name} attacked {monster.name} dealing {finalDamage} damage");
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (stats != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, stats.atRange);
        }
    }
}
