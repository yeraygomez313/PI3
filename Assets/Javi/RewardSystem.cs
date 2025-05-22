using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RewardSystem : MonoBehaviour
{
    //public Inventory inventory;
    public DeckManager deckManager;

    private int currentRound = 1;

    public void GiveReward()
    {
        List<PreparationCard> rewardOptions = new List<PreparationCard>();


        bool giveNewCard = true; 

        if (giveNewCard)
        {
            rewardOptions = GetRandomCards(3);
        }
        else
        {
            rewardOptions = GetUpgradableCardsFromDeck(3);
        }

        ShowRewardUI(rewardOptions);
    }

    private List<PreparationCard> GetRandomCards(int count)
    {
        //List<PreparationCard> pool = new List<PreparationCard>(inventory.MaxItems);
        List<PreparationCard> selected = new List<PreparationCard>();

        /*
        while (selected.Count < count && pool.Count > 0)
        {
            int index = Random.Range(0, pool.Count);
            Card chosen = pool[index].Clone();

            // En rondas altas, posibilidad de +1 de mejora
            if (currentRound >= 8 && Random.value < 0.4f)
                chosen.upgradeLevel = 1;

            selected.Add(chosen);
            pool.RemoveAt(index);
        }*/

        return selected;
    }

    private List<PreparationCard> GetUpgradableCardsFromDeck(int count)
    {
        var upgradable = deckManager.CardList
            //.Where(c => c.upgradeLevel < 2)
            .OrderBy(_ => Random.value)
            .Take(count)
            //.Select(c => c.Clone())
            .ToList();

        return upgradable;
    }

    public void SelectCard(PreparationCard selectedCard)
    {
        PreparationCard existing = deckManager.CardList.FirstOrDefault(c => c.name == selectedCard.name);

        if (existing != null)
        {
            //existing.Upgrade();
        }
        else
        {
            deckManager.CardList.Add(selectedCard);
        }

        currentRound++;
    }

    private void ShowRewardUI(List<PreparationCard> options)
    {
        Debug.Log("Mostrar 3 opciones al jugador.");
    }
}
