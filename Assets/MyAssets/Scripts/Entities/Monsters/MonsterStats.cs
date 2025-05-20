using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStats", menuName = "PI3/MonsterStats")]
public class MonsterStats : ScriptableObject
{
    public Sprite Icon;
    public float maxHealth;
    public float attack;
    public float defense;
    public float atSpeed;
    public float mvSpeed;
    public float atRange;
    public float scale;
}
