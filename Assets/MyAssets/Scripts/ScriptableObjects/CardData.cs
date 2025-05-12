using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public int ManaCost { get; private set; }
    [field: SerializeField] public GameObject MonsterPrefab { get; private set; }
    [field: SerializeField] public GameObject DeploymentPreviewPrefab { get; private set; }

    // Temporal
    [field: SerializeField] public List<CardLevelData> CardLevelDataList { get; private set; } = new();
}

[CreateAssetMenu(fileName = "CardLevelData", menuName = "Scriptable Objects/CardLevelData")]
public class CardLevelData : ScriptableObject
{
    public MonsterStats MonsterStats;
}

public class CardInstance
{
    public Guid Guid { get; private set; }
    public CardData Card { get; private set; }
    public int Level { get; private set; }
    public Sprite Icon => Card.Icon;
    public int ManaCost => Card.ManaCost;
    public GameObject MonsterPrefab => Card.MonsterPrefab;
    public GameObject DeploymentPreviewPrefab => Card.DeploymentPreviewPrefab;

    public void Initialize(CardData card, int level)
    {
        Guid = Guid.NewGuid();
        Card = card;
        Level = level;
    }

    public CardLevelData GetCardLevelData()
    {
        return Card.CardLevelDataList[Level];
    }

    public bool CanBeUpgraded()
    {
        if (Level < Card.CardLevelDataList.Count - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Upgrade()
    {
        if (CanBeUpgraded())
        {
            Level++;
        }
        else
        {
            Debug.LogWarning($"Card {Card.name} is already at max level.");
        }
    }
}
