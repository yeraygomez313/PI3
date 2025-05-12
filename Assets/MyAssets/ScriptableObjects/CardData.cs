using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public Sprite Icon;
    public int ManaCost;
    public GameObject MonsterPrefab;
    public GameObject DeploymentPreviewPrefab;

    public List<CardLevelData> CardLevelDataList;
}

[CreateAssetMenu(fileName = "CardLevelData", menuName = "Scriptable Objects/CardLevelData")]
public class CardLevelData : ScriptableObject
{

}

public class CardInstance
{
    public Guid Guid;
    public CardData Card;
    public int Level;

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
}
