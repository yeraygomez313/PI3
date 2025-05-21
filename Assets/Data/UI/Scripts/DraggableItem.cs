using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Canvas))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Draggable Item")]
    [field:SerializeField] public virtual ItemInstance ItemInstance { get; protected set; }
    public ItemSlot AssignedItemSlot { get; protected set; }
    public Inventory Inventory => AssignedItemSlot?.Inventory;

    [SerializeField] protected CanvasGroup staticVisuals;
    [SerializeField] protected CanvasGroup dragVisuals;

    protected GraphicRaycaster mainCanvasGraphicRaycaster;
    protected RectTransform rectTransform;
    protected Vector2 initialLocalPosition;
    protected bool isBeingDragged = false;
    protected bool interactable = true;

    [HideInInspector] public UnityEvent<DraggableItem> OnBeginItemDrag; // communication with the inventory
    [HideInInspector] public UnityEvent<DraggableItem> OnEndItemDrag; // communication with the inventory
    [HideInInspector] public UnityEvent<ItemSlot> OnSlotAssigned;
    [HideInInspector] public UnityEvent<DraggableItem> OnClicked; // communication with the inventory

    protected virtual void Awake()
    {
        mainCanvasGraphicRaycaster = GetComponentInParent<CanvasScaler>().GetComponent<GraphicRaycaster>();
        rectTransform = GetComponent<RectTransform>();
        dragVisuals.alpha = 0f;
    }

    public virtual void SetItem(ItemInstance newItem)
    {
        ItemInstance = newItem;
        // change visuals
    }

    protected virtual void Update()
    {
        if (isBeingDragged)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rectTransform.position = mousePos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData)) return;

        BeginDragBehavior(eventData);
    }

    protected virtual void BeginDragBehavior(PointerEventData eventData)
    {
        if (AssignedItemSlot != null)
        {
            initialLocalPosition = Vector2.zero;
        }
        else
        {
            initialLocalPosition = rectTransform.localPosition;
        }

        isBeingDragged = true;
        staticVisuals.alpha = 0f;
        dragVisuals.alpha = 1f;
        OnBeginItemDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData) || !isBeingDragged) return;

        DragBehavior(eventData);
    }

    protected virtual void DragBehavior(PointerEventData eventData)
    {
        // Empty for now
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData) || !isBeingDragged) return;

        EndDragBehavior(eventData);
    }

    protected virtual void EndDragBehavior(PointerEventData eventData)
    {
        // Check if the item is dropped on a valid slot
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = mainCanvasGraphicRaycaster;
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            ItemSlot slot = result.gameObject.GetComponentInParent<ItemSlot>();

            if (slot != null && slot != AssignedItemSlot)
            {
                // Valid slot found, assign the item to it
                slot.RequestItemAssignation(this);
                break;
            }
        }

        // Reset position and visuals
        rectTransform.localPosition = initialLocalPosition;
        isBeingDragged = false;
        staticVisuals.alpha = 1f;
        dragVisuals.alpha = 0f;
        OnEndItemDrag?.Invoke(this);
    }

    public virtual void AssignItemSlot(ItemSlot slot)
    {
        OnSlotAssigned?.Invoke(slot);
        AssignedItemSlot = slot;
        rectTransform.SetParent(slot.transform);
        initialLocalPosition = Vector2.zero;
        rectTransform.localPosition = initialLocalPosition;
    }

    public virtual void UnassignItemSlot()
    {
        OnSlotAssigned?.Invoke(null);
        AssignedItemSlot = null;
        rectTransform.SetParent(transform);
        Vector2 randomDir = Random.insideUnitCircle.normalized * Random.Range(40f, 60f);
        rectTransform.localPosition += (Vector3)randomDir;
    }

    protected virtual bool CanBeDragged(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && interactable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsOverAssignedItemSlot(PointerEventData eventData)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster raycaster = mainCanvasGraphicRaycaster;
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            ItemSlot slot = result.gameObject.GetComponentInParent<ItemSlot>();

            if (AssignedItemSlot != null && AssignedItemSlot == slot)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasSlotAssigned()
    {
        return AssignedItemSlot != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBeingDragged || !interactable) return;

        OnClicked?.Invoke(this);
    }
}
