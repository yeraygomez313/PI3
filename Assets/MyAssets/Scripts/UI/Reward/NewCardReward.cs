using UnityEngine;

public class NewCardReward : MonoBehaviour
{
    [SerializeField] private CardVisuals cardVisuals;
    private CardInstance cardInstance;

    public void Initialize(CardInstance card)
    {
        cardInstance = card;
        cardVisuals.SetCard(cardInstance.GetMonsterStats());
    }

    public void AddNewCard()
    {
        RewardManager.Instance.AddCardToDeck(cardInstance);
    }
}
