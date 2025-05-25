using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroController : LivingEntity
{
    public HeroStats stats;
    public LayerMask enemyLayer;

    [SerializeField] private List<AbilityData> abilities;

    private float attackCooldown = 0f;

    private LivingEntity monsterController;

    [SerializeField] private LocalForceAvoidance avoidance;
    private LocalForceAvoidance closestTarget;

    private float initialScale;

    private Quaternion lookAt = new Quaternion(0,0,0,1);

    [SerializeField] private GameObject projectilePrefab;

    private void Start()
    {
        currentHealth = stats.maxHealth;
        initialScale = transform.localScale.x;
        
    }

    private void Awake()
    {
        avoidance.SetMovementSpeed(stats.mvSpeed);
    }

    private void Update()
    {
        if (IsDead) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, stats.atRange, enemyLayer);

        if (enemies.Length > 0)
        {
            ChaseClosestTarget();
            lookAt.x = transform.position.x - closestTarget.transform.position.x;
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
        /*GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (monsterController.transform.position - transform.position).normalized;
        projectile.GetComponent<Projectile>().Initialize(direction, stats.attack, false);*/
        abilities[0].InstantiateAbility(this, lookAt, stats.attack);
    }

    private void ChaseClosestTarget()
    {
        closestTarget = ChunkManager.Instance.GetClosestTarget(avoidance);

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
