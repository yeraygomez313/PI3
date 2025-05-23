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
    }

    public void SetCombatCardList(List<CardInstance> cardlists)
	{
        combatCardlist = cardlists;
	}

    public List<CardInstance> GetUpgradableCards()
    {
        return combatCardlist.Where(card => card.CanBeUpgraded()).ToList();
    }

    public void UpgradeCard(CardInstance card)
    {
        if (card.CanBeUpgraded())
        {
            card.Upgrade();
        }
        else
        {
            Debug.LogWarning("Card cannot be upgraded");
        }
    }
}
