using System;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] ItemList itemDatabase;
    public PlayerInitialize initialize;

    public InventoryUI inventoryUI;
    public int inventorySlots = 30;
    public InventorySlotData[] items;

    public UnityEvent<Item, int> OnItemAdded;

    void Awake()
    {
        items = new InventorySlotData[inventorySlots];
    }

    public bool AddItem(Item newItem, int quantity = 1)
    {
        Debug.Log(quantity);
        OnItemAdded?.Invoke(newItem, quantity);

        if (newItem is Currency)
        {
            Player player = GetComponentInParent<Player>();
            if (player != null) player.CoinCollected(quantity); return true;
        }

        if (TryAutoEquip(newItem)) return true;
        if (TryStackItem(newItem, quantity)) return true;

        // Find empty slot
        int emptySlotIndex = Array.FindIndex(items, x => x == null);
        if (TryFindEmptySlot(emptySlotIndex)) return true;

        // Place item in empty slot
        items[emptySlotIndex] = new InventorySlotData(newItem, quantity);
        inventoryUI.UpdateUI();
        initialize.SaveInventory(newItem, emptySlotIndex, quantity);
        return true;
    }

    bool TryAutoEquip(Item newItem)
    {
        if (newItem is Equipment equipmentItem)
        {
            int slotIndex = (int)equipmentItem.equipmentType;

            if (equipmentManager.currentEquipment[slotIndex] == null)
            {
                equipmentManager.Equip(equipmentItem);
                return true;
            }
        }

        return false;
    }

    bool TryStackItem(Item newItem, int quantity)
    {
        if (newItem.IsStackable)
        {
            int existingIndex = Array.FindIndex(items, x => x != null && x.item.name == newItem.name);
            if (existingIndex != -1)
            {
                items[existingIndex].quantity += quantity;
                inventoryUI.UpdateUI();
                initialize.SaveInventory(newItem, existingIndex, items[existingIndex].quantity);
                return true;
            }
        }

        return false;
    }

    bool TryFindEmptySlot(int emptySlotIndex)
    {
        if (emptySlotIndex == -1)
        {
            Debug.Log("Inventory full");
            return false;
        }

        return false;
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

                if (parts.Length == 2 &&
                    !string.IsNullOrWhiteSpace(parts[0]) &&
                    int.TryParse(parts[1], out int quantity))
                {
                    string itemName = parts[0];

                    Item template = itemDatabase.GetItemByName(itemName);
                    if (template != null)
                    {
                        items[i] = new InventorySlotData(template, quantity);
                    }
                    else
                    {
                        Debug.LogWarning($"Item '{itemName}' not found in ItemDatabase.");
                        items[i] = null; // force clear if invalid item
                    }
                }
                else
                {
                    Debug.LogWarning($"Malformed inventory string for key: {key}");
                    items[i] = null;
                }
            }
            else
            {
                items[i] = null;
            }
        }

        inventoryUI.UpdateUI();
    }

    public bool AddItemToSlot(Item item, int quantity, int targetSlot)
    {
        if (items[targetSlot] == null)
        {
            items[targetSlot] = new InventorySlotData(item, quantity);
            initialize.SaveInventory(item, targetSlot, quantity);
            return true;
        }

        return false;
    }

    public void StackButton()
    {
        for (int i = 0; i < items.Length; i++)
        {
            InventorySlotData sourceSlot = items[i];
            if (sourceSlot == null || !sourceSlot.item.IsStackable)
                continue;

            for (int j = i + 1; j < items.Length; j++)
            {
                InventorySlotData targetSlot = items[j];
                if (targetSlot == null)
                    continue;

                if (targetSlot.item.name == sourceSlot.item.name)
                {
                    // Combine quantity into source
                    sourceSlot.quantity += targetSlot.quantity;

                    // Clear target slot
                    items[j] = null;
                    initialize.SaveInventory(null, j, 0); // Clear saved slot
                }
            }

            // Save updated source slot
            initialize.SaveInventory(sourceSlot.item, i, sourceSlot.quantity);
        }

        inventoryUI.UpdateUI();
    }
}
