using System;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private ItemList itemDatabase;
    [SerializeField] PlayerInitialize initialize;

    [SerializeField] InventoryUI inventoryUI;
    public int inventorySlots = 30;
    public InventorySlotData[] items;

    void Awake()
    {
        items = new InventorySlotData[inventorySlots];
    }

    public bool AddItem(Item newItem)
    {
        // Check if the item is stackable and already in inventory
        if (newItem.IsStackable)
        {
            int existingIndex = Array.FindIndex(items, x => x != null && x.item.name == newItem.name);
            if (existingIndex != -1)
            {
                items[existingIndex].quantity++;
                inventoryUI.UpdateUI();
                initialize.SaveInventory(newItem, existingIndex, items[existingIndex].quantity);
                return true;
            }
        }

        // Find empty slot
        int emptySlotIndex = Array.FindIndex(items, x => x == null);
        if (emptySlotIndex == -1)
        {
            Debug.Log("Inventory full");
            return false;
        }

        // Place item in empty slot
        items[emptySlotIndex] = new InventorySlotData(newItem, 1);
        inventoryUI.UpdateUI();
        initialize.SaveInventory(newItem, emptySlotIndex, 1);
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

            initialize.SaveInventory(removedItem, itemIndex, 1);
        }

        inventoryUI.UpdateUI();
    }

    public void LoadInventory()
    {
        string prefix = initialize.CharacterNumber;

        for (int i = 0; i < inventorySlots; i++)
        {
            string key = $"{prefix}InventorySlot_{i}";

            if (PlayerPrefs.HasKey(key))
            {
                string saved = PlayerPrefs.GetString(key);
                string[] parts = saved.Split('|');

                if (parts.Length == 2)
                {
                    string itemName = parts[0];
                    int quantity = int.Parse(parts[1]);

                    Item template = itemDatabase.GetItemByName(itemName);
                    if (template != null)
                    {
                        items[i] = new InventorySlotData(template, quantity);
                    }
                    else
                    {
                        Debug.LogWarning($"Item '{itemName}' not found in ItemDatabase.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Malformed inventory string for key: {key}");
                }
            }
            else
            {
                items[i] = null;
            }
        }

        inventoryUI.UpdateUI();
    }
}
