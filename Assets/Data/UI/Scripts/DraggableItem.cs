using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemData data;
    [SerializeField] private CanvasGroup staticVisuals;
    [SerializeField] private CanvasGroup dragVisuals;

    private GraphicRaycaster mainCanvasGraphicRaycaster;
    private RectTransform rectTransform;
    private ItemSlot assignedItemSlot;
    private Vector2 initialLocalPosition;
    private bool isBeingDragged;

    protected virtual void Awake()
    {
        mainCanvasGraphicRaycaster = GetComponentInParent<CanvasScaler>().GetComponent<GraphicRaycaster>();
        rectTransform = GetComponent<RectTransform>();
        assignedItemSlot = GetComponentInParent<ItemSlot>();
    }

    public virtual void SetData(ItemData newData)
    {
        data = newData;
        // Update visuals based on the new data
    }

    protected virtual void Update()
    {
        if (isBeingDragged)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rectTransform.position = mousePos;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData)) return;

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
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = mousePos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!CanBeDragged(eventData)) return;

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
    }

    protected bool CanBeDragged(PointerEventData eventData)
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
}
