using UnityEngine;

public class MonsterAI : LivingEntity
{
    public MonsterStats stats;

    [SerializeField] private LocalForceAvoidance avoidance;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool canShoot = false;

    private LivingEntity heroController;
    private float attackCooldown;
    private int remainingShots = 10;
    private bool inRangedMode = false;

    private void Awake()
    {
        if (stats != null)
        {
            Initialize(stats);
        }
    }

    public void Initialize(MonsterStats monsterStats)
    {;
        GetComponent<SpriteRenderer>().sprite = monsterStats.Icon; //DEBUG
        stats = monsterStats;
        currentHealth = monsterStats.maxHealth;
        avoidance.SetMovementSpeed(monsterStats.mvSpeed);
        ChaseClosestTarget();
    }

    private void Update()
    {
        if (IsDead) return;

        attackCooldown -= Time.deltaTime;
        if (heroController == null || heroController.IsDead)
        {
            ChaseClosestTarget();
            return;
        }

        float distance = Vector2.Distance(transform.position, heroController.transform.position);

        // --- MODO DE DISPARO ---
        if (canShoot && remainingShots > 0 && distance <= stats.atRange + 10f && distance > stats.atRange)
        {
            if (!inRangedMode)
            {
                // Se queda quieto
                avoidance.SetTarget(transform.position);
                inRangedMode = true;
            }

            TryShootHero();
            return;
        }

        // --- MODO MELEE ---
        if (distance <= stats.atRange)
        {
            if (heroController == null || heroController.IsDead) return;
            Debug.Log("Attacking (melee)");
            TryAttackHero();
        }
        else
        {
            if (inRangedMode)
            {
                // Si estaba disparando y se quedó sin tiros, vuelve al modo melee
                if (remainingShots <= 0)
                {
                    Debug.Log("Out of arrows, chasing target");
                    inRangedMode = false;
                    ChaseClosestTarget();
                }
            }
            else
            {
                ChaseClosestTarget();
            }
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

    private void TryShootHero()
    {
        if (attackCooldown <= 0f)
        {
            Debug.Log("Shooting arrow");
            remainingShots--;
            attackCooldown = 1f / stats.atSpeed;

            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Vector2 direction = (heroController.transform.position - transform.position).normalized;
                projectile.GetComponent<Projectile>().Initialize(direction, stats.attack, true);
            }
            else
            {
                // Si no hay proyectil, aplica daño directo como simulación
                heroController.TakeDamage(stats.attack);
            }
        }
    }
}
