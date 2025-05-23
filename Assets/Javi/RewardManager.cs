using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] private List<CardData> allCards;
    [SerializeField] private CanvasGroup optionsGroup;
    [SerializeField] private CanvasGroup rewardGroup;
    [SerializeField] private GameObject upgradeOption;
    [SerializeField] private int rewardCount = 3;
    [SerializeField] private GameObject newCardRewardPrefab;
    [SerializeField] private GameObject upgradeCardRewardPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvas.worldCamera = Camera.main;

        if (DeckManager.Instance.GetUpgradableCards().Count > 0)
        {
            upgradeOption.SetActive(true);
        }
        else
        {
            upgradeOption.SetActive(false);
        }
    }

    public void CreateNewCardRewards()
    {
        for (int i = 0; i < rewardCount; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            CardData randomCard = allCards[randomIndex];

            CardInstance newCard = new CardInstance();
            newCard.Initialize(randomCard, 0);

            GameObject newCardReward = Instantiate(newCardRewardPrefab, rewardGroup.transform);
            NewCardReward newCardRewardScript = newCardReward.GetComponent<NewCardReward>();
            newCardRewardScript.Initialize(newCard);
        }

        ShowRewardGroup();
    }

    public void CreateUpgradeCardRewards()
    {
        List<CardInstance> upgradableCards = DeckManager.Instance.GetUpgradableCards();

        for (int i = 0; i < rewardCount && i < upgradableCards.Count; i++)
        {
            CardInstance randomCard = upgradableCards[i];
            GameObject upgradeCardReward = Instantiate(upgradeCardRewardPrefab, rewardGroup.transform);
            UpgradeCardReward upgradeCardRewardScript = upgradeCardReward.GetComponent<UpgradeCardReward>();
            upgradeCardRewardScript.Initialize(randomCard);
        }

        ShowRewardGroup();
    }

    public void AddCardToDeck(CardInstance selectedCard)
    {
        DeckManager.Instance.AddCardToDeck(selectedCard);
        HideRewardGroup();
        DOVirtual.DelayedCall(1f, () =>
        {
            GameManager.Instance.StartNewRound();
        });
    }

    public void UpdradeCardInDeck(CardInstance selectedCard)
    {
        DeckManager.Instance.UpgradeCard(selectedCard);
        HideRewardGroup();
        DOVirtual.DelayedCall(1f, () =>
        {
            GameManager.Instance.StartNewRound();
        });
    }

    private void ShowRewardGroup()
    {
        optionsGroup.alpha = 0;
        optionsGroup.interactable = false;
        optionsGroup.blocksRaycasts = false;
        rewardGroup.alpha = 1;
        rewardGroup.interactable = true;
        rewardGroup.blocksRaycasts = true;
    }

    private void HideRewardGroup()
    {
        rewardGroup.alpha = 0;
        rewardGroup.interactable = false;
        rewardGroup.blocksRaycasts = false;
    }

    //public void GiveReward()
    //{
    //    List<CardInstance> rewardOptions = new List<CardInstance>();


    //    bool giveNewCard = true;

    //    if (giveNewCard)
    //    {
    //        rewardOptions = GetRandomCards(3);
    //    }
    //    else
    //    {
    //        rewardOptions = GetUpgradableCardsFromDeck(3);
    //    }

    //    ShowRewardUI(rewardOptions);
    //}

    //public void SelectCard(CardInstance selectedCard)
    //{
    //    CardInstance existing = deckManager.CardList.FirstOrDefault(c => c.name == selectedCard.name);

    //    if (existing != null)
    //    {
    //        //existing.Upgrade();
    //    }
    //    else
    //    {
    //        deckManager.CardList.Add(selectedCard);
    //    }

    //    currentRound++;
    //}
}
