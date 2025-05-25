using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed = 10f;
    private float damage;
    private bool isMonster;

    public void Initialize(Vector2 dir, float dmg, bool isMnstr)
    {
        direction = dir.normalized;
        damage = dmg;
        isMonster = isMnstr;
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMonster)
        {
            HeroController hero = collision.GetComponent<HeroController>();

            if (hero != null)
            {
                LivingEntity entity = hero.GetComponent<LivingEntity>();
                if (entity != null)
                {
                    entity.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
        else
        {
            MonsterAI monster = collision.GetComponent<MonsterAI>();

            if (monster != null)
            {
                LivingEntity entity = monster.GetComponent<LivingEntity>();
                if (entity != null)
                {
                    entity.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}
