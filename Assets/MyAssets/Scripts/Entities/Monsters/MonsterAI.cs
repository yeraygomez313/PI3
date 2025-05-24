using UnityEngine;

public class MonsterAI : LivingEntity
{
    public MonsterStats stats;

    [SerializeField] private LocalForceAvoidance avoidance;
    private LivingEntity heroController;
    private float attackCooldown;

    private void Awake()
    {
        if (stats != null)
        {
            Initialize(stats);
        }
    }

    public void Initialize(MonsterStats monsterStats)
    {
        GetComponent<SpriteRenderer>().sprite = monsterStats.Icon; //DEBUG
        stats = monsterStats;
        currentHealth = monsterStats.maxHealth;
        avoidance.SetMovementSpeed(monsterStats.mvSpeed);
        ChaseClosestTarget();
    }

    private void Update()
    {
        if (IsDead) return;

        ChaseClosestTarget();
        attackCooldown -= Time.deltaTime;
        if (heroController == null || heroController.IsDead) return;

        float distance = Vector2.Distance(transform.position, heroController.transform.position);

        if (distance <= stats.atRange)
        {
            if (heroController == null || heroController.IsDead) return;
            Debug.Log("Attacking");
            TryAttackHero();
            //avoidance.SetStatic(true);
        }
        else
        {
            Debug.Log("Searching");
            //avoidance.SetStatic(false);
        }
    }

    private void ChaseClosestTarget()
    {
        LocalForceAvoidance closestTarget = ChunkManager.Instance.GetClosestTarget(avoidance);

        if (closestTarget == null)
        {
            Debug.Log("No target found");
            return;
        }

        avoidance.SetTarget(closestTarget.transform.position);
        heroController = closestTarget.GetComponent<LivingEntity>();
    }


    private void MoveTowardsHero()
    {
        Vector2 direction = (heroController.transform.position - transform.position).normalized;
        transform.position += (Vector3)(direction * stats.mvSpeed * Time.deltaTime);
    }

    private void TryAttackHero()
    {
        if (attackCooldown <= 0f)
        {
            float finalDamage = Mathf.Max(1, stats.attack /*- heroController.stats.defense*/);
            heroController.TakeDamage(finalDamage);
            attackCooldown = 1f / stats.atSpeed;
        }
    }
}
