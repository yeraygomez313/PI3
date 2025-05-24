using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TestHeroController : LivingEntity
{
    public HeroStats stats;
    public LayerMask enemyLayer;

    [SerializeField] private List<AbilityData> abilities;

    private float attackCooldown = 0f;

    private LivingEntity monsterController;

    [SerializeField] private LocalForceAvoidance avoidance;

    private float initialScale;

    private void Start()
    {
        currentHealth = stats.maxHealth;
        initialScale = transform.localScale.x;
    }

    private void Update()
    {
        if (IsDead) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, stats.atRange, enemyLayer);
        
        if (enemies.Length > 0)
        {
            ChaseClosestTarget();
        }

        attackCooldown -= Time.deltaTime;

        if (enemies.Length > 0 && attackCooldown <= 0f)
        {
            AutoAttackEnemies();
            attackCooldown = 1f / abilities[0].Cooldown * stats.atSpeed;
        }
    }

    private void AutoAttackEnemies()
    {
        abilities[0].InstantiateAbility(this, transform.rotation, stats.attack);
        
    }
    private void ChaseClosestTarget()
    {
        LocalForceAvoidance closestTarget = ChunkManager.Instance.GetClosestTarget(avoidance);

        if (closestTarget == null)
        {
            //Debug.Log("No target found");
            return;
        }

        avoidance.SetTarget(closestTarget.transform.position);
        monsterController = closestTarget.GetComponent<LivingEntity>();
        Vector3 newScale = new Vector3(Mathf.Sign(transform.position.x - closestTarget.transform.position.x) * initialScale, initialScale, initialScale);
        transform.localScale = newScale;
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
