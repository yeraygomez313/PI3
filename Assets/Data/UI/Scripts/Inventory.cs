using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [field:SerializeField] public int MaxItems { get; protected set; } = 4;
    [SerializeField] protected GameObject itemSlotPrefab;
    [SerializeField] protected GameObject draggableItemPrefab;
    [SerializeField] protected List<ItemData> initialInventory = new();
    protected List<ItemSlot> inventorySlots = new();

    protected virtual void Awake()
    {
        for (int i = 0; i < MaxItems; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
            itemSlot.name = "ItemSlot_" + i;
            ItemSlot itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
            itemSlotComponent.OnItemAssigned.AddListener(OnItemAssigned);
            inventorySlots.Add(itemSlotComponent);

            if (initialInventory.Count > i)
            {
                GameObject item = Instantiate(draggableItemPrefab.gameObject, itemSlot.transform);
                item.name = "Item_" + i;
                item.GetComponent<RectTransform>().SetParent(itemSlot.transform);
                DraggableItem itemComponent = item.GetComponent<DraggableItem>();
                itemSlotComponent.AssignItem(itemComponent);
                ItemInstance itemInstance = new ItemInstance();
                itemInstance.Initialize(initialInventory[i]);
                itemComponent.SetItem(itemInstance);
            }
        }
    }

    public virtual void AddItem(ItemInstance itemInstance)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].AssignedItem == null)
            {
                AddItem(itemInstance, i);
                return;
            }
        }
        Debug.Log("Inventory is full");
    }

    public virtual void AddItem(ItemInstance itemInstance, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            Debug.LogError("Invalid slot index");
            return;
        }

        ItemSlot slot = inventorySlots[slotIndex];
        if (slot.AssignedItem != null)
        {
            Debug.Log("Slot already occupied");
            return;
        }

        GameObject item = Instantiate(draggableItemPrefab, slot.transform);
        item.name = "Item_" + slotIndex;
        item.GetComponent<RectTransform>().SetParent(slot.transform);
        DraggableItem itemComponent = item.GetComponent<DraggableItem>();
        slot.AssignItem(itemComponent);
        itemComponent.SetItem(itemInstance);
    }

    protected virtual void OnItemAssigned(DraggableItem item)
    {
        // Logic to handle when an item is assigned to a slot
        // This could involve updating the UI, checking for item limits, etc.
        Debug.Log("ItemInstance assigned: " + item.name);
    }

    public List<DraggableItem> GetInventoryDraggableItems()
    {
        List<DraggableItem> inventoryData = new List<DraggableItem>();

        foreach (ItemSlot slot in inventorySlots)
        {
            if (slot.AssignedItem != null)
            {
                inventoryData.Add(slot.AssignedItem);
            }
        }
        return inventoryData;
    }

    public List<ItemInstance> GetInventoryItemInstances()
    {
        List<ItemInstance> inventoryData = new List<ItemInstance>();

        foreach (ItemSlot slot in inventorySlots)
        {
            if (slot.AssignedItem != null)
            {
                inventoryData.Add(slot.AssignedItem.ItemInstance);
            }
        }

        return inventoryData;
    }
}
