using System;
using UnityEngine;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour
{
    [SerializeField] private Image cardIcon;

    public void SetCard(MonsterStats monsterStats)
    {
        cardIcon.sprite = monsterStats.Icon;
    }
}
