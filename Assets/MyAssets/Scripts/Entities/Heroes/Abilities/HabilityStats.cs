using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HabilityStats", menuName = "PI3/HabilityStats")]
public class HabilityStats : ScriptableObject
{
    public string habilityName;
    public float instantDamage;
    public float tickDamage;
    public float cooldown;
    public float duration;
}
