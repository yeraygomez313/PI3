using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class Inventory : MonoBehaviour
{
    [field:SerializeField] public int MaxItems { get; protected set; } = 4;
    [field: SerializeField] public bool AllowsSlotSwapping { get; protected set; } = true;
    [SerializeField] protected bool flexible = false;
    [SerializeField] protected Inventory linkedInventory;
    [SerializeField] protected GameObject itemSlotPrefab;
    [SerializeField] protected GameObject draggableItemPrefab;
    [SerializeField] protected List<ItemData> initialInventory = new();
    protected List<ItemSlot> inventorySlots = new();

    public UnityEvent<DraggableItem> OnItemAddedToInventory;
    public UnityEvent<DraggableItem> OnItemRemovedFromInventory;

    protected virtual void Awake()
    {
        if (flexible)
        {
            for (int i = 0; i < initialInventory.Count; i++)
            {
                ItemInstance itemInstance = new ItemInstance();
                itemInstance.Initialize(initialInventory[i]);

                AddItemSlotWithItemInstance(itemInstance, i);
            }
        }
        else
        {
            for (int i = 0; i < MaxItems; i++)
            {
                // Instantiate item slots and add them to the inventory
                GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
                itemSlot.name = "ItemSlot_" + i;
                ItemSlot itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
                itemSlotComponent.OnItemAssigned.AddListener(OnItemAssigned);
                itemSlotComponent.OnItemUnassigned.AddListener(OnItemUnassigned);
                inventorySlots.Add(itemSlotComponent);

                if (initialInventory.Count > i) // Create new items for each slot if there is an initial inventory
                {
                    // Instantiate item
                    GameObject draggableItem = Instantiate(draggableItemPrefab.gameObject, itemSlot.transform);
                    draggableItem.name = "Item_" + i;

                    // Create item instance
                    ItemInstance itemInstance = new ItemInstance();
                    itemInstance.Initialize(initialInventory[i]);

                    // Set item instance of created draggable item
                    DraggableItem draggableItemComponent = draggableItem.GetComponent<DraggableItem>();
                    draggableItemComponent.SetItem(itemInstance);

                    // Assign item to slot
                    itemSlotComponent.RequestItemAssignation(draggableItemComponent);
                }
            }
        }
    }

    private void AddItemSlotWithItemInstance(ItemInstance itemInstance, int i)
    {
        ItemSlot itemSlotComponent = AddItemSlot();

        // Instantiate item
        GameObject draggableItem = Instantiate(draggableItemPrefab.gameObject, itemSlotComponent.transform);
        draggableItem.name = "Item_" + i;

        // Set item instance of created draggable item
        DraggableItem draggableItemComponent = draggableItem.GetComponent<DraggableItem>();
        draggableItemComponent.SetItem(itemInstance);

        // Assign item to slot
        itemSlotComponent.RequestItemAssignation(draggableItemComponent);
    }

    private ItemSlot AddItemSlot()
    {
        // Instantiate item slots and add them to the inventory
        GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
        itemSlot.name = "ItemSlot_" + inventorySlots.Count;
        ItemSlot itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
        itemSlotComponent.OnItemAssigned.AddListener(OnItemAssigned);
        itemSlotComponent.OnItemUnassigned.AddListener(OnItemUnassigned);
        inventorySlots.Add(itemSlotComponent);
        return itemSlotComponent;
    }

    public virtual void AddItems(List<ItemInstance> itemInstances)
    {
        foreach (ItemInstance itemInstance in itemInstances)
        {
            AddItem(itemInstance);
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

        if (flexible)
        {
            AddItemSlotWithItemInstance(itemInstance, inventorySlots.Count);
        }
        else
        {
            Debug.Log("Inventory is full");
        }
    }

    public ItemSlot GetFirstEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].AssignedItem == null)
            {
                return inventorySlots[i];
            }
        }

        return null;
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

        // Instantiate item
        GameObject draggableItem = Instantiate(draggableItemPrefab.gameObject, slot.transform);
        draggableItem.name = "Item_" + slotIndex;

        // Set item instance of created draggable item
        DraggableItem draggableItemComponent = draggableItem.GetComponent<DraggableItem>();
        draggableItemComponent.SetItem(itemInstance);

        // Assign item to slot
        slot.RequestItemAssignation(draggableItemComponent);
    }

    protected virtual void OnItemAssigned(DraggableItem item)
    {
        item.OnClicked.AddListener(OnItemClicked);
        OnItemAddedToInventory?.Invoke(item);
    }

    protected virtual void OnItemUnassigned(DraggableItem item)
    {
        item.OnClicked.RemoveListener(OnItemClicked);
        OnItemRemovedFromInventory?.Invoke(item);
    }

    protected virtual void OnItemClicked(DraggableItem item)
    {
        if (!AllowsSlotSwapping) return;

        if (linkedInventory != null)
        {
            ItemSlot emptySlot = linkedInventory.GetFirstEmptySlot();

            if (emptySlot != null)
            {
                emptySlot.RequestItemAssignation(item);
            }
            else if (linkedInventory.flexible)
            {
                emptySlot = linkedInventory.AddItemSlot();
                emptySlot.RequestItemAssignation(item);
            }
            else
            {
                Debug.Log("No empty slots in linked inventory");
            }
        }
        else
        {
            Debug.Log("No linked inventory");
        }
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
