using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    public static DeckManager Instance { get; private set; }

    [field:SerializeField] public List<CardInstance> cardlist { get; private set; } = new();
    public List<CardInstance> combatCardlist { get; private set; } = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); 
        }

        foreach (CardInstance card in cardlist)
        {
            card.Initialize(card.CardData, card.Level);
        }
    }

    public void SetCombatCardList(List<CardInstance> cardlists)
	{
        combatCardlist = cardlists;
	}

    public void AddCardToDeck(CardInstance card)
    {
        if (!cardlist.Contains(card))
        {
            cardlist.Add(card);
        }
        else
        {
            Debug.LogWarning("Card already in deck");
        }
    }

    public List<CardInstance> GetUpgradableCards()
    {
        return combatCardlist.Where(card => card.CanBeUpgraded()).ToList();
    }

    public void UpgradeCard(CardInstance card)
    {
        CardInstance cardToUpgrade = combatCardlist.FirstOrDefault(c => c.Guid == card.Guid);

        if (cardToUpgrade == null)
        {
            Debug.LogWarning("Card not found in combat card list");
            return;
        }

        if (cardToUpgrade.CanBeUpgraded())
        {
            cardToUpgrade.Upgrade();
        }
        else
        {
            Debug.LogWarning("Card cannot be upgraded");
        }
    }
}
