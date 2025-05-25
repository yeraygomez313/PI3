using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class DraggableCombatCard : DraggableItem
{
    public override ItemInstance ItemInstance
    {
        get => CardInstance;
        protected set => CardInstance = value as CardInstance;
    }
    public CardInstance CardInstance { get; private set; }

    [Header("Draggable Combat Card")]
    [SerializeField] private CardVisuals cardVisuals;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Color notEnoughManaColor;
    [SerializeField] private float cardUsedCooldown;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private Image deploymentFailedOverlay;
    [SerializeField] private float deploymentFailedOverlayInitialAlpha;
    [SerializeField] private float deploymentFailedSequenceDuration;
    [SerializeField] private float cardRotationSpeed = 180f;
    private bool rotatingCard = false;

    private CombatManager combatManager;
    private Sequence deploymentFailedSequence;
    public DeploymentPreview DeploymentPreviewObject { get; private set; } // CHANGE?

    private void Start()
    {
        combatManager = CombatManager.Instance;
        CombatManager.Instance.OnCardUsed.AddListener(OnCardUsed);
    }

    private void OnDisable()
    {
        combatManager.OnCardUsed.RemoveListener(OnCardUsed);
        if (DeploymentPreviewObject != null)
        {
            Destroy(DeploymentPreviewObject.gameObject);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (combatManager.HasEnoughMana(CardInstance.ManaCost))
        {
            manaCostText.color = Color.white;
        }
        else
        {
            manaCostText.color = notEnoughManaColor;
        }

        if (rotatingCard && isBeingDragged)
        {
            DeploymentPreviewObject.transform.Rotate(0, 0, cardRotationSpeed * Time.deltaTime);
        }
    }

    public override void SetItem(ItemInstance card)
    {
        if (card is not CardInstance cardInstance) return;

        CardInstance = cardInstance;

        cardVisuals.SetCard(cardInstance);
        manaCostText.text = cardInstance.ManaCost.ToString();

        if (DeploymentPreviewObject != null)
        {
            Destroy(DeploymentPreviewObject.gameObject);
        }
        DeploymentPreviewObject = Instantiate(cardInstance.DeploymentPreviewPrefab, dragVisuals.transform).GetComponent<DeploymentPreview>();
        DeploymentPreviewObject.GetComponent<RectTransform>().SetParent(dragVisuals.transform);
        DeploymentPreviewObject.SetCard(cardInstance);
    }

    private void OnCardUsed(DraggableCombatCard cardUsed)
    {
        DisableTemporarily(cardUsedCooldown);
    }

    public void DisableTemporarily(float duration)
    {
        interactable = false;
        cooldownOverlay.fillAmount = 1f;

        DOTween.To(() => cooldownOverlay.fillAmount, x => cooldownOverlay.fillAmount = x, 0f, duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                interactable = true;
            });
    }

    protected override void BeginDragBehavior(PointerEventData eventData)
    {
        base.BeginDragBehavior(eventData);
    }

    protected override void DragBehavior(PointerEventData eventData)
    {
        base.DragBehavior(eventData);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);

        bool canBeDeployed = combatManager.ValidateDeploymentPosition(mousePos) && combatManager.HasEnoughMana(CardInstance.ManaCost);

        if (IsOverAssignedItemSlot(eventData))
        {
            DeploymentPreviewObject.SetDeploymentState(DeploymentState.Canceled);
        }
        else 
        {
            if (canBeDeployed)
            {
                DeploymentPreviewObject.SetDeploymentState(DeploymentState.Allowed);
            }
            else
            {
                DeploymentPreviewObject.SetDeploymentState(DeploymentState.Forbidden);
            }
        }
    }

    protected override void EndDragBehavior(PointerEventData eventData)
    {
        if (!IsOverAssignedItemSlot(eventData))
        {
            if (CombatManager.Instance.TryToUseCard(this, DeploymentPreviewObject.transform.position))
            {
                // Handle successful deployment
            }
            else
            {
                DisableTemporarily(deploymentFailedSequenceDuration);

                deploymentFailedOverlay.GetComponent<CanvasGroup>().alpha = deploymentFailedOverlayInitialAlpha;

                deploymentFailedSequence = DOTween.Sequence();
                deploymentFailedSequence.Append(rectTransform.DOLocalMoveX(initialLocalPosition.x + 10f, deploymentFailedSequenceDuration / 4f)
                    .SetLoops(2, LoopType.Yoyo));
                deploymentFailedSequence.Append(rectTransform.DOLocalMoveX(initialLocalPosition.x - 10f, deploymentFailedSequenceDuration / 4f)
                    .SetLoops(2, LoopType.Yoyo));
                deploymentFailedSequence.Insert(0, deploymentFailedOverlay.GetComponent<CanvasGroup>().DOFade(0f, deploymentFailedSequenceDuration)
                    .SetEase(Ease.InQuad));
            }
        }

        base.EndDragBehavior(eventData);
    }

    private void OnRotateCard(InputValue value)
    {
        rotatingCard = value.isPressed;
    }
}
