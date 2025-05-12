using UnityEngine;

[CreateAssetMenu(fileName = "MonsterStats", menuName = "PI3/MonsterStats")]
public class MonsterStats : ScriptableObject
{
    public float maxHealth;
    public float attack;
    public float atSpeed;
    public float mvSpeed;
    public float atRange;
}
