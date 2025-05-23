using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "PI3/CardData")]
public class CardData : ItemData
{
    [field: SerializeField] public int ManaCost { get; private set; }
    [field: SerializeField] public GameObject MonsterPrefab { get; private set; }
    [field: SerializeField] public GameObject DeploymentPreviewPrefab { get; private set; }

    // Temporal
    [field: SerializeField] public List<MonsterStats> CardLevelDataList { get; private set; } = new();
}

[Serializable]
public class CardInstance : ItemInstance
{
    public override ItemData Data
    {
        get => CardData;
        protected set => CardData = value as CardData;
    }
    [field: SerializeField] public CardData CardData { get; private set; }

    [field: SerializeField] public int Level { get; private set; }
    public int ManaCost => CardData.ManaCost;
    public GameObject MonsterPrefab => CardData.MonsterPrefab;
    public GameObject DeploymentPreviewPrefab => CardData.DeploymentPreviewPrefab;

    public void Initialize(CardData card, int level)
    {
        base.Initialize(card);
        CardData = card;
        Level = level;
    }

    public MonsterStats GetMonsterStats()
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
