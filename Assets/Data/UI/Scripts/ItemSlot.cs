using UnityEngine;
using UnityEngine.Events;

public class ItemSlot : MonoBehaviour
{
    public DraggableItem AssignedItem { get; protected set; }

    [HideInInspector] public UnityEvent<DraggableItem> OnItemAssigned;

    public virtual void AssignItem(DraggableItem item)
    {
        AssignedItem = item;
        OnItemAssigned?.Invoke(item);
    }
}
