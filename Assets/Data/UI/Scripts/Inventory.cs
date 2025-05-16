using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxItems = 10;
    [SerializeField] private bool expandable = false;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private GameObject draggableItemPrefab;
    [SerializeField] private List<ItemData> initialInventory = new();

    private void Awake()
    {
        for (int i = 0; i < maxItems; i++)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, transform);
            itemSlot.name = "ItemSlot_" + i;
            ItemSlot itemSlotComponent = itemSlot.GetComponent<ItemSlot>();
            itemSlotComponent.OnItemAssigned.AddListener(OnItemAssigned);

            if (initialInventory.Count > i)
            {
                GameObject item = Instantiate(draggableItemPrefab.gameObject, itemSlot.transform);
                item.name = "Item_" + i;
                DraggableItem itemComponent = item.GetComponent<DraggableItem>();
                itemSlotComponent.AssignItem(itemComponent);
                itemComponent.SetData(initialInventory[i]);
            }
        }
    }

    private void OnItemAssigned(DraggableItem item)
    {
        // Logic to handle when an item is assigned to a slot
        // This could involve updating the UI, checking for item limits, etc.
        Debug.Log("Item assigned: " + item.name);
    }
}
