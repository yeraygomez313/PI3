using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BarbarianAttacks : MonoBehaviour
{
    public TestHeroController heroController;

    private void Awake()
    {
        CircleCollider2D collider = this.GetComponent<CircleCollider2D>();
        collider.radius = heroController.stats.atRange/40;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(transform.position, this.GetComponent<CircleCollider2D>().radius, heroController.enemyLayer))
        {
            MonsterAI monster = enemy.GetComponent<MonsterAI>();
            if (monster != null && !monster.IsDead)
            {
                float finalDamage = Mathf.Max(1, heroController.stats.attack - (heroController.stats.attack * monster.stats.defense / 100));
                monster.TakeDamage(finalDamage);
                Debug.Log($"{gameObject.name} attacked {monster.name} dealing {finalDamage} damage");
                break;
            }
        }
    }
}
