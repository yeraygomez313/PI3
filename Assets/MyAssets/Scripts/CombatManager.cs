using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private float cardCooldown = 1f;
    [SerializeField] private float spawnDelay = 0.1f;

    [SerializeField] private ManaBar manaBar;

    [SerializeField] private GameObject cardHolder;
    private List<DraggableCard> cardsInHand = new List<DraggableCard>();
    public DraggableCard SelectedCard { get; private set; }
    //[field:SerializeField] private CardInstance[] deck = new CardInstance[8];
    //private Queue<CardInstance> cardQueue = new Queue<CardInstance>();

    [SerializeField] private LayerMask forbiddenZoneMask;
    [HideInInspector] public UnityEvent OnCardSelected;
    [HideInInspector] public UnityEvent OnCardDeselected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var card in cardHolder.GetComponentsInChildren<DraggableCard>())
        {
            cardsInHand.Add(card);
            card.SetCardInstance(/*cardInstance*/);
        }

        //load deck
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

        if (manaBar.ConsumeMana(card.ManaCost))
        {
            StartCoroutine(SpawnUnits(card));

            foreach (var c in cardsInHand)
            {
                c.DisableTemporarily(cardCooldown);
            }

            // dequeue next card
            // queue used card
            // set DraggableCard to next card using SetCardInstance

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
            GameObject monster = Instantiate(card.MonsterPrefab, spawnPoint.position, Quaternion.identity);
            monster.transform.localScale = spawnPoint.localScale; // Placeholder
            yield return spawnDelayWait; // Placeholder for spawn delay
        }
    }
}
