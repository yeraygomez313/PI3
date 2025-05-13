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

    [SerializeField] private ManaBar manaBar;
    [SerializeField] private TimeBar timeBar;

    [SerializeField] private GameObject cardHolder;
    private List<DraggableCard> cardsInHand = new List<DraggableCard>();
    [SerializeField] private LayerMask forbiddenZoneMask;

    private CardInstance[] deck = new CardInstance[8];
    public DraggableCard SelectedCard { get; private set; }
    private Queue<CardInstance> cardQueue = new Queue<CardInstance>();

    [HideInInspector] public UnityEvent OnCardSelected;
    [HideInInspector] public UnityEvent OnCardDeselected;

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

        foreach (var card in cardHolder.GetComponentsInChildren<DraggableCard>())
        {
            cardsInHand.Add(card);
        }

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].SetCardInstance(deck[i]);
        }

        for (int i = cardsInHand.Count; i < deck.Length; i++)
        {
            cardQueue.Enqueue(deck[i]);
        }

        timeBar.OnTimeUp.AddListener(EndGame);

        //
    }

    private void EndGame()
    {
        Debug.Log("Game Over");
    }

    public void CardSelected(DraggableCard card)
    {
        SelectedCard = card;
        OnCardSelected?.Invoke();
    }

    public void CardDeselected()
    {
        SelectedCard = null;
        OnCardDeselected?.Invoke();
    }

    public bool UseCard(DraggableCard card, Vector2 position)
    {
        if (ValidateDeploymentPosition(position) == false)
        {
            Debug.Log("Invalid deployment position.");
            return false;
        }

        if (manaBar.ConsumeMana(card.CardInstance.ManaCost))
        {
            StartCoroutine(SpawnUnits(card));

            foreach (var c in cardsInHand)
            {
                c.DisableTemporarily(cardCooldown);
            }

            CardInstance nextCard = cardQueue.Dequeue();
            cardQueue.Enqueue(card.CardInstance);
            card.SetCardInstance(nextCard);

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
        return manaBar.HasEnoughMana(manaCost);
    }

    private IEnumerator SpawnUnits(DraggableCard card)
    {
        var spawnDelayWait = new WaitForSeconds(spawnDelay);
        var spawnPoints = card.DeploymentPreviewObject.GetSpawnPoints();

        foreach (var spawnPoint in spawnPoints)
        {
            GameObject monster = Instantiate(card.CardInstance.MonsterPrefab, spawnPoint.position, Quaternion.identity);
            monster.transform.localScale = spawnPoint.localScale; // Placeholder
            yield return spawnDelayWait; // Placeholder for spawn delay
        }
    }
}
