using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Draggable Item")]
    [field:SerializeField] public virtual ItemInstance ItemInstance { get; protected set; }
    [SerializeField] protected CanvasGroup staticVisuals;
    [SerializeField] protected CanvasGroup dragVisuals;

    protected GraphicRaycaster mainCanvasGraphicRaycaster;
    protected RectTransform rectTransform;
    protected ItemSlot assignedItemSlot;
    protected Vector2 initialLocalPosition;
    protected bool isBeingDragged = false;
    protected bool interactable = true;

    [HideInInspector] public UnityEvent<DraggableItem> OnBeginItemDrag;
    [HideInInspector] public UnityEvent<DraggableItem> OnEndItemDrag;

    protected virtual void Awake()
    {
        mainCanvasGraphicRaycaster = GetComponentInParent<CanvasScaler>().GetComponent<GraphicRaycaster>();
        rectTransform = GetComponent<RectTransform>();
        assignedItemSlot = GetComponentInParent<ItemSlot>();
        dragVisuals.alpha = 0f;
    }

    public virtual void SetItem(ItemInstance newItem)
    {
        ItemInstance = newItem;
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
        if (assignedItemSlot != null)
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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        rectTransform.position = mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData) || !isBeingDragged) return;

        EndDragBehavior(eventData);
    }

    protected virtual void EndDragBehavior(PointerEventData eventData)
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
            ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();

            if (slot != null && slot != assignedItemSlot)
            {
                rectTransform.SetParent(slot.transform);
                initialLocalPosition = Vector2.zero;
                assignedItemSlot = slot;
                slot.AssignItem(this);
                break;
            }
        }

        rectTransform.localPosition = initialLocalPosition;
        isBeingDragged = false;
        staticVisuals.alpha = 1f;
        dragVisuals.alpha = 0f;
        OnEndItemDrag?.Invoke(this);
    }

    protected bool CanBeDragged(PointerEventData eventData)
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
            ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();

            if (assignedItemSlot != null && assignedItemSlot == slot)
            {
                return true;
            }
        }

        return false;
    }
}
