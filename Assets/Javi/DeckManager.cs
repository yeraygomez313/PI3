using System.Collections.Generic;
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
}
