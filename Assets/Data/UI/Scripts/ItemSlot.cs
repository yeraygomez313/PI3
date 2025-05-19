using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class ItemSlot : MonoBehaviour
{
    public DraggableItem AssignedItem { get; protected set; }
    public Inventory Inventory { get; protected set; }

    [HideInInspector] public UnityEvent<DraggableItem> OnItemAssigned;
    [HideInInspector] public UnityEvent<DraggableItem> OnItemUnassigned;

    protected virtual void Awake()
    {
        Inventory = GetComponentInParent<Inventory>();
    }

    public virtual bool RequestItemAssignation(DraggableItem newItem)
    {
        if (newItem.HasSlotAssigned())
        {
            if (!Inventory.AllowsSlotSwapping) return false;

            if (AssignedItem != null)
            {
                var tempItem = AssignedItem;
                AssignedItem.UnassignItemSlot();
                if (!newItem.AssignedItemSlot.RequestItemAssignation(tempItem))
                {
                    Debug.LogError("The requested slot does not have a reference to its item, but the item does have a reference to the slot.");
                    return false;
                }
            }

            AssignItem(newItem);
            return true;
        }
        else
        {
            if (AssignedItem != null)
            {
                if (!Inventory.AllowsSlotSwapping) return false;
                AssignedItem.UnassignItemSlot();
            }

            AssignItem(newItem);
            return true;
        }
    }

    protected virtual void AssignItem(DraggableItem newItem)
    {
        AssignedItem = newItem;
        newItem.AssignItemSlot(this);
        newItem.OnSlotAssigned.AddListener(AssignedItemChangedSlots);
        OnItemAssigned?.Invoke(newItem);
    }

    protected virtual void AssignedItemChangedSlots(ItemSlot itemSlot)
    {
        if (itemSlot != this)
        {
            AssignedItem.OnSlotAssigned.RemoveListener(AssignedItemChangedSlots);
            OnItemUnassigned?.Invoke(AssignedItem);
            AssignedItem = null;
        }
    }
}
