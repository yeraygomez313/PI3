using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PreparationMode : MonoBehaviour
{

    public Inventory inventory;
    public Inventory chosenCardsInventory;
    public static PreparationMode Instance { get; private set; }

    private PreparationCard[] cardPlayerList = new PreparationCard[5];


    [SerializeField] private CanvasGroup startCombatButton;
    [SerializeField] private Canvas canvasPreparation;
    //[SerializeField] private int roundcount = 0;


    public bool CanPlayerPlaceMoreCards => cardPlayerList.Count(element => element != null) < 5;


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

        chosenCardsInventory.OnItemAddedToInventory.AddListener(CheckCards);
        chosenCardsInventory.OnItemRemovedFromInventory.AddListener(CheckRemove);

        canvasPreparation.worldCamera = Camera.main;

        GameObject heroFormation = GameManager.Instance.GetHeroFormation();
        GameObject formationInstance = Instantiate(heroFormation, Vector3.zero, Quaternion.identity, ChunkManager.Instance.transform);
        Time.timeScale = 0f;
    }

	private void OnDisable()
	{
        chosenCardsInventory.OnItemAddedToInventory.RemoveListener(CheckCards);
        chosenCardsInventory.OnItemRemovedFromInventory.RemoveListener(CheckRemove);
    }

	private void Start()
    {
        if (DeckManager.Instance == null)
        {
            return;
        }

        var cardList = DeckManager.Instance.cardlist;
		foreach (var card in cardList)
		{
            inventory.AddItem(card);
		}

    }

    private void CheckCards(DraggableItem arg0)
    {
        var _invent = chosenCardsInventory.GetInventoryItemInstances();
        if (_invent.Count >= chosenCardsInventory.MaxItems)
        {
            startCombatButton.alpha = 1;
            startCombatButton.interactable = true;
            startCombatButton.blocksRaycasts = true;
        }
    }

    public void CheckButton()
	{
        var _invent = chosenCardsInventory.GetInventoryItemInstances();
        var derivedList = _invent.Cast<CardInstance>().ToList();
        DeckManager.Instance.SetCombatCardList(derivedList);
        GameManager.Instance.StartCombat();
    }

    private void CheckRemove(DraggableItem arg0)
	{
        startCombatButton.alpha = 0;
        startCombatButton.interactable = false;
        startCombatButton.blocksRaycasts = false;
	}
}
