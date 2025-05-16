using UnityEngine;
using UnityEngine.Events;

public class ItemSlot : MonoBehaviour
{
    private DraggableItem assignedItem;

    [HideInInspector] public UnityEvent<DraggableItem> OnItemAssigned;

    public void AssignItem(DraggableItem item)
    {
        // Logic to assign the item to this slot
        // This could involve setting the item's parent, updating its position, etc.+
        assignedItem = item;
        OnItemAssigned?.Invoke(item);
    }
}
