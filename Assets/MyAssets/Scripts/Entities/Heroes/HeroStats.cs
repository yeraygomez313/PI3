using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroStats", menuName = "PI3/HeroStats")]
public class HeroStats : ScriptableObject
{
    public HeroClass heroClass;
    public float maxHealth;
    public float defense;
    public float attack;
    public float atSpeed;
    public float mvSpeed;
    public float atRange;
}

public enum HeroClass
{
    mage,
    paladin,
    barbarian
}