using System;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] Player player;
    [SerializeField] ItemList itemDatabase;
    [SerializeField] PlayerQuest playerquests;
    public PlayerInitialize initialize;

    public InventoryUI inventoryUI;
    public int inventorySlots = 30;
    public InventorySlotData[] items;

    public UnityEvent<Item, int> OnItemAdded;

    void Awake()
    {
        items = new InventorySlotData[inventorySlots];
    }

    public bool AddItem(Item newItem, int quantity = 1, bool isUnEquip = false)
    {
        if (TryCollectCurrency(newItem, quantity))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        if (TryAutoEquip(newItem, quantity))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        if (TryStackItem(newItem, quantity))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        // Find empty slot
        int emptySlotIndex = Array.FindIndex(items, x => x == null);
        if (emptySlotIndex == -1)
        {
            Debug.Log("Inventory full");
            return false;
        }

        // Place item in empty slot
        items[emptySlotIndex] = new InventorySlotData(newItem, quantity);
        inventoryUI.UpdateUI();
        if (!isUnEquip) OnItemAdded?.Invoke(newItem, quantity);
        initialize.SaveInventory(newItem, emptySlotIndex, quantity);

        CheckIfItemIsForQuest(newItem, quantity);
        return true;
    }

    void CheckIfItemIsForQuest(Item newItem, int quantity)
    {
        foreach (QuestProgress progress in playerquests.activeQuests)
        {
            if (progress.state != QuestState.InProgress) continue;

            foreach (QuestObjective objective in progress.objectives)
            {
                if (objective.type == ObjectiveType.Collect && objective.ObjectiveID == newItem.ITEM_ID && !objective.IsCompleted)
                {
                    playerquests.UpdateObjective(ObjectiveType.Collect, newItem.ITEM_ID, quantity);
                    break;
                }
            }
        }
    }

    bool TryCollectCurrency(Item newItem, int quantity)
    {
        if (newItem is Currency)
        {
            OnItemAdded?.Invoke(newItem, quantity);
            player.CoinCollected(quantity); return true;
        }

        return false;
    }

    bool TryAutoEquip(Item newItem, int quantity)
    {
        if (newItem is Equipment equipmentItem)
        {
            int slotIndex = (int)equipmentItem.equipmentType;

            if (equipmentManager.currentEquipment[slotIndex] == null)
            {
                equipmentManager.Equip(equipmentItem);
                OnItemAdded?.Invoke(newItem, quantity);
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
                OnItemAdded?.Invoke(newItem, quantity);
                initialize.SaveInventory(newItem, existingIndex, items[existingIndex].quantity);
                return true;
            }
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

    public int GetFreeSlotCount()
    {
        int free = 0;
        foreach (var slot in items)
        {
            if (slot == null)
                free++;
        }
        return free;
    }
}
