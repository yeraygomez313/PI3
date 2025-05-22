using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private float cardCooldown = 1f;
    [SerializeField] private float spawnDelay = 0.1f;

    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup combatGroup;
    [SerializeField] private float cardDraggedAlpha = 0.4f;

    [SerializeField] private ManaBar manaBar;
    [SerializeField] private TimeBar timeBar;

    [SerializeField] private Inventory cardInventory;
    [SerializeField] private LayerMask forbiddenZoneMask;

    private CardInstance[] deck = new CardInstance[8];
    public DraggableCombatCard SelectedCard { get; private set; }
    private Queue<CardInstance> cardQueue = new Queue<CardInstance>();

    [HideInInspector] public UnityEvent<DraggableCombatCard> OnCardUsed;
    [HideInInspector] public UnityEvent<DraggableCombatCard> OnCardSelected;
    [HideInInspector] public UnityEvent<DraggableCombatCard> OnCardDeselected;

    [Header("Debug")]
    [SerializeField] private List<CardData> cardData = new List<CardData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = new CardInstance();
            deck[i].Initialize(cardData[i], 0);
        }

        deck = deck.OrderBy(x => Random.value).ToArray();

        timeBar.OnTimeUp.AddListener(EndGame);
        canvas.worldCamera = Camera.main;

        // listen to heroes dying
    }

    private void Start()
    {
        for (int i = 0; i < cardInventory.MaxItems; i++)
        {
            cardInventory.AddItem(deck[i]);
        }

        foreach (var card in cardInventory.GetInventoryDraggableItems())
        {
            card.OnBeginItemDrag.AddListener(CardSelected);
            card.OnEndItemDrag.AddListener(CardDeselected);
        }

        //foreach (var card in cardHolder.GetComponentsInChildren<DraggableCombatCard>())
        //{
        //    cardsInHand.Add(card);
        //}

        //for (int i = 0; i < cardsInHand.Count; i++)
        //{
        //    cardsInHand[i].SetCardInstance(deck[i]);
        //}

        for (int i = cardInventory.MaxItems; i < deck.Length; i++)
        {
            cardQueue.Enqueue(deck[i]);
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over");
    }

    private void CardSelected(DraggableItem card)
    {
        combatGroup.alpha = cardDraggedAlpha;
        OnCardSelected?.Invoke(card as DraggableCombatCard);
    }

    private void CardDeselected(DraggableItem card)
    {
        combatGroup.alpha = 1f;
        OnCardDeselected?.Invoke(card as DraggableCombatCard);
    }

    public bool TryToUseCard(DraggableCombatCard card, Vector2 position)
    {
        if (ValidateDeploymentPosition(position) == false)
        {
            Debug.Log("Invalid deployment position.");
            return false;
        }

        if (manaBar.ConsumeAmount(card.CardInstance.ManaCost))
        {
            List<Vector3> spawnPoints = card.DeploymentPreviewObject.GetSpawnPoints();
            StartCoroutine(SpawnUnits(spawnPoints, card.CardInstance.MonsterPrefab));

            OnCardUsed?.Invoke(card);
            CardInstance nextCard = cardQueue.Dequeue();
            cardQueue.Enqueue(card.CardInstance);
            card.SetItem(nextCard);

            return true;
        }

        return false;
    }

    public bool ValidateDeploymentPosition(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, forbiddenZoneMask);

        if (hit != null)
        {
            return false;
        }

        return true;
    }

    public bool HasEnoughMana(int manaCost)
    {
        return manaBar.HasEnoughAmount(manaCost);
    }

    private IEnumerator SpawnUnits(List<Vector3> spawnPoints, GameObject monsterPrefab)
    {
        var spawnDelayWait = new WaitForSeconds(spawnDelay);

        foreach (var spawnPoint in spawnPoints)
        {
            GameObject monster = Instantiate(monsterPrefab, spawnPoint, Quaternion.identity, ChunkManager.Instance.transform);
            yield return spawnDelayWait; // Placeholder for spawn delay
        }
    }
}
