using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float dragAlpha = 0.4f;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image monsterIconImage;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Color notEnoughManaColor;
    [SerializeField] private Image cooldownImage;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 initialPosition;
    private CombatManager combatManager;
    public DeploymentPreview DeploymentPreviewObject { get; private set; }

    //Temporal, replace when scriptable object is finished
    [Header("Placeholder")]
    public int ManaCost;
    public GameObject MonsterPrefab;
    public Sprite MonsterIcon;
    public GameObject DeploymentPreview;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        SetCardInstance(/*cardInstance*/);
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;
    }

    private void Update()
    {
        if (combatManager.HasEnoughMana(ManaCost))
        {
            manaCostText.color = Color.white;
        }
        else
        {
            manaCostText.color = notEnoughManaColor;
        }
    }

    public void SetCardInstance(/*CardInstance card*/)
    {
        monsterIconImage.sprite = MonsterIcon;
        manaCostText.text = ManaCost.ToString();
        Destroy(DeploymentPreviewObject);
        DeploymentPreviewObject = Instantiate(DeploymentPreview, transform.position, Quaternion.identity).GetComponent<DeploymentPreview>();
        DeploymentPreviewObject.gameObject.SetActive(false);
    }

    public void DisableTemporarily(float duration)
    {
        Debug.Log("Card disabled temporarily for " + duration + " seconds.");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        cooldownImage.fillAmount = 1f;

        DOTween.To(() => cooldownImage.fillAmount, x => cooldownImage.fillAmount = x, 0f, duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });

    }

    private bool CanDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag(eventData)) return;

        initialPosition = rectTransform.localPosition;
        canvasGroup.alpha = dragAlpha;
        DeploymentPreviewObject.gameObject.SetActive(true);
        CombatManager.Instance.CardSelected(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag(eventData)) return;

        Vector2 mousePos = eventData.position;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = mousePos;
        DeploymentPreviewObject.transform.position = mousePos;
        bool canBeDeployed = combatManager.ValidateDeploymentPosition(mousePos) && combatManager.HasEnoughMana(ManaCost);
        DeploymentPreviewObject.SetDeploymentAllowed(canBeDeployed);
    }

    public void UpdatePositionOnCameraMovement(Vector2 mousePos)
    {
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = mousePos;
        bool canBeDeployed = combatManager.ValidateDeploymentPosition(mousePos) && combatManager.HasEnoughMana(ManaCost);
        DeploymentPreviewObject.SetDeploymentAllowed(canBeDeployed);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag(eventData)) return;

        DeploymentPreviewObject.gameObject.SetActive(false);

        if (CombatManager.Instance.UseCard(this, rectTransform.position))
        {
            // Handle successful deployment
            Debug.Log("Card deployed successfully!");
        }
        else
        {
            // Return to initial position if deployment is invalid
            Debug.Log("Invalid deployment position. Returning to initial position.");
        }

        rectTransform.localPosition = initialPosition;
        canvasGroup.alpha = 1f;
        CombatManager.Instance.CardDeselected();
    }
}
