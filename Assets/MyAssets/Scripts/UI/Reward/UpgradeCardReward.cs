using UnityEngine;

public class UpgradeCardReward : MonoBehaviour
{
    [SerializeField] private CardVisuals cardVisuals;
    private CardInstance cardInstance;

    public void Initialize(CardInstance card)
    {
        cardInstance = card;
        cardVisuals.SetCard(cardInstance);
    }

    public void UpgradeCard()
    {
        RewardManager.Instance.UpdradeCardInDeck(cardInstance);
    }
}
