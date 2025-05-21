using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BarbarianRockbreakerHability : MonoBehaviour
{
    public TestHeroController heroController;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (Collider2D enemy in Physics2D.OverlapBoxAll(transform.position, this.GetComponent<BoxCollider2D>().size, heroController.enemyLayer))
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
