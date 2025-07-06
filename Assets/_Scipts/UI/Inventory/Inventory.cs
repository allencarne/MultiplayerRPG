using System;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] PlayerInitialize initialize;
    [SerializeField] InventoryUI inventoryUI;
    public int inventorySlots = 30;
    public Item[] items;

    void Awake()
    {
        items = new Item[inventorySlots];
    }

    public bool AddItem(Item newItem)
    {
        if (Array.FindIndex(items, x => x == null) == -1)
        {
            Debug.Log("Not enough room.");
            return false;
        }

        // Find the first empty slot in the inventory
        int emptySlotIndex = Array.FindIndex(items, x => x == null);

        if (emptySlotIndex != -1)
        {
            // Check if the item is stackable
            if (newItem.IsStackable)
            {
                // Check if the item already exists in the inventory
                int existingItemIndex = Array.FindIndex(items, x => x == newItem);
                if (existingItemIndex != -1)
                {
                    // If the item exists, increase its quantity
                    items[existingItemIndex].Quantity++;
                    inventoryUI.UpdateUI();

                    initialize.SaveInventory(newItem, emptySlotIndex);

                    return true;
                }
            }

            // If the item is not stackable or doesn't exist in the inventory, add it to an empty slot
            newItem.Quantity = 1;
            items[emptySlotIndex] = newItem;

            initialize.SaveInventory(newItem, emptySlotIndex);
        }
        else
        {
            Debug.Log("Inventory is full.");
            return false; // Return false if no empty slot is found
        }

        inventoryUI.UpdateUI();
        return true;
    }

    public void RemoveItem(Item removedItem)
    {
        // Find the index of the item in the inventory
        int itemIndex = Array.IndexOf(items, removedItem);
        if (itemIndex != -1)
        {
            // Remove the item from the inventory by setting its slot to null
            items[itemIndex] = null;

            initialize.SaveInventory(removedItem, itemIndex);
        }

        inventoryUI.UpdateUI();
    }
}
