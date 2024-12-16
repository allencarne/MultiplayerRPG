using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;

    public int inventorySlots = 30;
    public Item[] items;

    void Awake()
    {
        items = new Item[inventorySlots];
    }

    public void AddItem(Item newItem)
    {
        if (Array.FindIndex(items, x => x == null) == -1)
        {
            Debug.Log("Not enough room.");
        }

        // Find the first empty slot in the inventory
        int emptySlotIndex = Array.FindIndex(items, x => x == null);

        if (emptySlotIndex != -1)
        {
            // Check if the item is stackable
            if (newItem.isStackable)
            {
                // Check if the item already exists in the inventory
                int existingItemIndex = Array.FindIndex(items, x => x == newItem);
                if (existingItemIndex != -1)
                {
                    // If the item exists, increase its quantity
                    items[existingItemIndex].quantity++;
                    inventoryUI.UpdateUI();
                }
            }

            // If the item is not stackable or doesn't exist in the inventory, add it to an empty slot
            newItem.quantity = 1;
            items[emptySlotIndex] = newItem;
        }

        inventoryUI.UpdateUI();
    }

    public void RemoveItem(Item removedItem)
    {
        // Find the index of the item in the inventory
        int itemIndex = Array.IndexOf(items, removedItem);
        if (itemIndex != -1)
        {
            // Remove the item from the inventory by setting its slot to null
            items[itemIndex] = null;
        }
    }
}
