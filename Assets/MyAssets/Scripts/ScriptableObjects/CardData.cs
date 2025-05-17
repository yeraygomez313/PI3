using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ItemData
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

public class CardInstance : ItemInstance
{
    public override ItemData Data
    {
        get => CardData;
        protected set => CardData = value as CardData;
    }
    public CardData CardData { get; private set; }

    public int Level { get; private set; }
    public Sprite Icon => CardData.Icon;
    public int ManaCost => CardData.ManaCost;
    public GameObject MonsterPrefab => CardData.MonsterPrefab;
    public GameObject DeploymentPreviewPrefab => CardData.DeploymentPreviewPrefab;

    public void Initialize(CardData card, int level)
    {
        CardData = card;
        Level = level;
    }

    public CardLevelData GetCardLevelData()
    {
        return CardData.CardLevelDataList[Level];
    }

    public bool CanBeUpgraded()
    {
        if (Level < CardData.CardLevelDataList.Count - 1)
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
            Debug.LogWarning($"CardData {CardData.name} is already at max level.");
        }
    }
}
