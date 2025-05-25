using System;
using UnityEngine;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour
{
    [SerializeField] private Image cardIcon;
    [SerializeField] private Image border;
    [SerializeField] private Color levelOneBorderColor;
    [SerializeField] private Color levelTwoBorderColor;
    [SerializeField] private Color levelThreeBorderColor;

    public void SetCard(CardInstance card)
    {
        cardIcon.sprite = card.GetMonsterStats().Icon;

        switch (card.Level)
        {
            case 0:
                border.color = levelOneBorderColor;
                break;
            case 1:
                border.color = levelTwoBorderColor;
                break;
            case >= 2:
                border.color = levelThreeBorderColor;
                break;
        }
    }
}
