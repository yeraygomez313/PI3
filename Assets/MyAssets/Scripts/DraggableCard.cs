using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image monsterIconImage;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Color notEnoughManaColor;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image deploymentFailedOverlay;
    [SerializeField] private float deploymentFailedOverlayInitialAlpha;
    [SerializeField] private float deploymentFailedSequenceDuration;

    public RectTransform RectTransform { get; private set; }
    private CanvasGroup canvasGroup;
    private Vector2 initialPosition;
    private CombatManager combatManager;
    private Sequence deploymentFailedSequence;
    public DeploymentPreview DeploymentPreviewObject { get; private set; }

    public CardInstance CardInstance { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        combatManager = CombatManager.Instance;
    }

    private void Update()
    {
        if (combatManager.HasEnoughMana(CardInstance.ManaCost))
        {
            manaCostText.color = Color.white;
        }
        else
        {
            manaCostText.color = notEnoughManaColor;
        }
    }

    public void SetCardInstance(CardInstance card)
    {
        CardInstance = card;
        monsterIconImage.sprite = card.Icon;
        manaCostText.text = card.ManaCost.ToString();
        if (DeploymentPreviewObject != null)
        {
            Destroy(DeploymentPreviewObject.gameObject);
        }
        DeploymentPreviewObject = Instantiate(card.DeploymentPreviewPrefab, transform.position, Quaternion.identity).GetComponent<DeploymentPreview>();
        DeploymentPreviewObject.gameObject.SetActive(false);
    }

    public void DisableTemporarily(float duration)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        cooldownOverlay.fillAmount = 1f;

        DOTween.To(() => cooldownOverlay.fillAmount, x => cooldownOverlay.fillAmount = x, 0f, duration).SetEase(Ease.Linear)
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

        if (deploymentFailedSequence != null)
        {
            deploymentFailedSequence.Complete();
        }

        initialPosition = RectTransform.localPosition;
        DeploymentPreviewObject.gameObject.SetActive(true);
        CombatManager.Instance.CardSelected(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag(eventData)) return;

        UpdatePosition(eventData.position);
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(screenPos);
        RectTransform rectTransform = GetComponent<RectTransform>();
        //rectTransform.position = mousePos;
        DeploymentPreviewObject.transform.position = mousePos;

        bool canBeDeployed = combatManager.ValidateDeploymentPosition(mousePos) && combatManager.HasEnoughMana(CardInstance.ManaCost);
        bool isInside = IsInside(screenPos);

        if (isInside)
        {
            DeploymentPreviewObject.SetDeploymentCanceled();
        }
        else
        {
            DeploymentPreviewObject.SetDeploymentAllowed(canBeDeployed);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag(eventData)) return;

        DeploymentPreviewObject.gameObject.SetActive(false);

        if (!IsInside(eventData.position))
        {
            if (CombatManager.Instance.TryToUseCard(this, DeploymentPreviewObject.transform.position))
            {
                // Handle successful deployment
            }
            else
            {
                // Return to initial position if deployment is invalid

                DisableTemporarily(deploymentFailedSequenceDuration);
                deploymentFailedOverlay.GetComponent<CanvasGroup>().alpha = deploymentFailedOverlayInitialAlpha;

                deploymentFailedSequence = DOTween.Sequence();

                deploymentFailedSequence.Append(RectTransform.DOLocalMoveX(initialPosition.x + 10f, deploymentFailedSequenceDuration / 4f)
                    .SetLoops(2, LoopType.Yoyo));
                deploymentFailedSequence.Append(RectTransform.DOLocalMoveX(initialPosition.x - 10f, deploymentFailedSequenceDuration / 4f)
                    .SetLoops(2, LoopType.Yoyo));
                deploymentFailedSequence.Insert(0, deploymentFailedOverlay.GetComponent<CanvasGroup>().DOFade(0f, deploymentFailedSequenceDuration)
                    .SetEase(Ease.InQuad));
            }
        }

        RectTransform.localPosition = initialPosition;
        CombatManager.Instance.CardDeselected();
    }

    private bool IsInside(Vector2 screenPos)
    {
        bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
            RectTransform,
            screenPos,
            Camera.main
        );

        return isInside;
    }
}
