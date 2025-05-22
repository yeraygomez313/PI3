using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    public static DeckManager Instance { get; private set; }


    private List<PreparationCard> cardList = new List<PreparationCard>();


    public List<PreparationCard> CardList
    {
        get { return cardList; }
        set { cardList = value; }
    }


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
}
