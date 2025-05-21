using System.Threading;
using UnityEngine;

public class TestHeroController : LivingEntity
{
    public HeroStats stats;
    public LayerMask enemyLayer;

    public CircleCollider2D attackCollider;
    public BoxCollider2D rockbreakerCollider;

    private float attackCooldown = 0f;

    private MonsterAI closestMonster = null;

    private void Start()
    {
        currentHealth = stats.maxHealth;
    }

    private void Update()
    {
        if (IsDead) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, stats.atRange, enemyLayer);
        if (enemies.Length > 0)
        {
            closestMonster = enemies[0].GetComponent<MonsterAI>();
            this.transform.rotation = closestMonster.transform.rotation.normalized;
        }

        attackCooldown -= Time.deltaTime;

        if (enemies.Length > 0 && attackCooldown <= 0f)
        {
            AutoAttackEnemies(/*enemies*/);
            attackCooldown = 1f / stats.atSpeed;
        }
        attackCollider.enabled = false;
    }

    private void AutoAttackEnemies(/*Collider2D[] enemies*/)
    {

        attackCollider.enabled = true;

        /*Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, stats.atRange, enemyLayer);
        Debug.Log("Autoattack: detected " + enemies.Length + " enemies");

        foreach (var enemy in enemies)
        {
            MonsterAI monster = enemy.GetComponent<MonsterAI>();
            if (monster != null && !monster.IsDead)
            {
                float finalDamage = Mathf.Max(1, stats.attack - (stats.attack * monster.stats.defense / 100));
                monster.TakeDamage(finalDamage);
                Debug.Log($"{gameObject.name} attacked {monster.name} dealing {finalDamage} damage");
                break;
            }
        }*/
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
