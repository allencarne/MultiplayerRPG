using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] PlayerQuest playerquests;
    [SerializeField] PlayerExperience playerExperience;
    public ItemList itemDatabase;
    public PlayerStats Stats;
    public PlayerSave Save;

    public InventoryUI inventoryUI;
    public int inventorySlots = 30;
    public InventorySlotData[] items;

    public UnityEvent OnInventoryChanged;
    public UnityEvent<Item, int> OnItemAdded;
    public UnityEvent OnCoinsChanged;

    private void OnEnable()
    {
        playerExperience.OnLevelUp.AddListener(OnLevelUp);
    }

    private void OnDisable()
    {
        playerExperience.OnLevelUp.RemoveListener(OnLevelUp);
    }

    void Awake()
    {
        items = new InventorySlotData[inventorySlots];
    }

    public bool AddItem(Item newItem, int quantity = 1, ItemRarity rarity = ItemRarity.Uncommon, ItemQuality quality = ItemQuality.Normal, List<StatModifier> modifiers = null, bool isUnEquip = false)
    {
        if (TryCollectCurrency(newItem, quantity))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        if (TryAutoEquip(newItem, quantity, rarity, quality, modifiers))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        if (TryStackItem(newItem, quantity))
        {
            CheckIfItemIsForQuest(newItem, quantity);
            return true;
        }

        // Find the first empty slot in the inventory
        int emptySlotIndex = Array.FindIndex(items, x => x == null);

        // If no empty slot is found, the inventory is full
        if (emptySlotIndex == -1)
        {
            Debug.Log("Inventory full");
            return false;
        }

        // Place item in empty slot
        items[emptySlotIndex] = new InventorySlotData(newItem, quantity, rarity, quality, modifiers);

        // Update the UI to reflect the new item
        inventoryUI.UpdateUI();

        // Invoke the OnItemAdded event if the item was not unequipped
        if (!isUnEquip) OnItemAdded?.Invoke(newItem, quantity);

        // Save the inventory state using the slot data
        Save.SaveInventory(items[emptySlotIndex], emptySlotIndex);

        // Check if the item is related to any active quests
        CheckIfItemIsForQuest(newItem, quantity);

        // return true to indicate the item was successfully added
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
            CoinCollected(quantity); return true;
        }

        return false;
    }

    bool TryAutoEquip(Item newItem, int quantity, ItemRarity rarity, ItemQuality quality, List<StatModifier> modifiers = null)
    {
        if (newItem is Equipment equipmentItem)
        {
            // slotIndex corresponds to the EquipmentType enum value
            int slotIndex = (int)equipmentItem.equipmentType;

            if (equipmentManager != null && equipmentManager.currentEquipment != null && slotIndex >= 0 && slotIndex < equipmentManager.currentEquipment.Length)
            {
                if (equipmentManager.currentEquipment[slotIndex] == null)
                {
                    // construct a slot instance using the runtime data so Equip can handle dynamic modifiers/rarity/quality
                    InventorySlotData newSlot = new InventorySlotData(equipmentItem, quantity, rarity, quality, modifiers);

                    // Equip expects InventorySlotData (see EquipmentManager.cs)
                    if (equipmentManager.Equip(newSlot))
                    {
                        OnItemAdded?.Invoke(equipmentItem, quantity);
                        return true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("EquipmentManager or its currentEquipment array is null or slotIndex out of range in TryAutoEquip.");
            }
        }

        return false;
    }

    bool TryStackItem(Item newItem, int quantity)
    {
        // Check if the item is stackable
        if (newItem.IsStackable)
        {
            // Find the index of an existing stack of the same item
            int existingIndex = Array.FindIndex(items, x => x != null && x.item.name == newItem.name);

            // If an existing stack is found, increase its quantity
            if (existingIndex != -1)
            {
                // Increase the quantity of the existing stack
                items[existingIndex].quantity += quantity;

                // Update the UI to reflect the new quantity
                inventoryUI.UpdateUI();

                // Invoke the OnItemAdded event to notify listeners of the quantity change
                OnItemAdded?.Invoke(newItem, quantity);

                // Save the updated inventory state for the existing stack
                Save.SaveInventory(items[existingIndex], existingIndex);

                // Return true to indicate that the item was successfully stacked
                return true;
            }
        }

        // If the item is not stackable or no existing stack was found,
        // return false to indicate that the item could not be stacked
        return false;
    }

    public void RemoveItem(Item removedItem)
    {
        // Find the index of the item to be removed in the inventory
        int itemIndex = Array.IndexOf(items, removedItem);

        // If the item is found in the inventory, remove it
        if (itemIndex != -1)
        {
            // Clear the inventory slot by setting it to null
            items[itemIndex] = null;

            // Save the updated inventory state for the removed item
            Save.SaveInventory(null, itemIndex);
        }

        // Update the UI to reflect the removal of the item
        inventoryUI.UpdateUI();
    }

    public bool AddItemToSlot(Item item, int quantity, int targetSlot)
    {
        // Validate the target slot index
        if (items[targetSlot] == null)
        {
            // Create a new InventorySlotData for the specified slot
            items[targetSlot] = new InventorySlotData(item, quantity, item.ItemRarity, item.ItemQuality);

            // save the inventory state for the newly added item in the specified slot
            Save.SaveInventory(items[targetSlot], targetSlot);

            // return true to indicate the item was successfully added to the specified slot
            return true;
        }

        // return false if the target slot is already occupied
        return false;
    }

    public void StackButton()
    {
        // Iterate through the inventory slots and combine stackable items of the same type, rarity, and quality
        for (int i = 0; i < items.Length; i++)
        {
            // Skip null slots and non-stackable items
            InventorySlotData sourceSlot = items[i];
            if (sourceSlot == null || !sourceSlot.item.IsStackable)
                continue;

            // Iterate through the remaining slots to find matching items to stack
            for (int j = i + 1; j < items.Length; j++)
            {
                // Skip null slots
                InventorySlotData targetSlot = items[j];
                if (targetSlot == null)
                    continue;

                // Check if the target slot has the same item, rarity, and quality as the source slot
                if (targetSlot.item.name == sourceSlot.item.name && targetSlot.rarity == sourceSlot.rarity && targetSlot.quality == sourceSlot.quality)
                {
                    // Combine quantities
                    sourceSlot.quantity += targetSlot.quantity;

                    // Clear the target slot after stacking
                    items[j] = null;

                    // save the updated inventory state for the target slot after clearing it
                    Save.SaveInventory(null, j);
                }
            }

            // save the updated inventory state for the source slot after stacking
            Save.SaveInventory(items[i], i);
        }

        // Update the inventory UI after stacking items
        inventoryUI.UpdateUI();
    }

    public void SortButton()
    {
        // Create a list of non-null inventory slots for sorting
        List<InventorySlotData> sorted = new List<InventorySlotData>();

        // Add non-null slots to the sorted list
        foreach (InventorySlotData slot in items)
        {
            // Only add non-null slots to the sorted list
            if (slot != null) sorted.Add(slot);
        }

        // Sort the list based on the defined criteria
        sorted.Sort((a, b) =>
        {
            // Sort by category first (weapons, equipment, consumables, collectables)
            int categoryA = GetSortCategory(a.item);
            int categoryB = GetSortCategory(b.item);

            // If categories are different, sort by category
            if (categoryA != categoryB)
                return categoryA.CompareTo(categoryB);

            // If both items are equipment, sort by level requirement (descending)
            if (a.item is Equipment equipA && b.item is Equipment equipB)
                return equipB.LevelRequirement.CompareTo(equipA.LevelRequirement);

            // If both items are consumables, sort by quantity (descending)
            return string.Compare(a.item.name, b.item.name, StringComparison.OrdinalIgnoreCase);
        });

        // Update the inventory slots with the sorted items and save the state
        for (int i = 0; i < items.Length; i++)
        {
            // Assign the sorted item to the inventory slot or null if there are no more sorted items
            items[i] = i < sorted.Count ? sorted[i] : null;

            // save the updated inventory state for each slot after sorting
            Save.SaveInventory(items[i], i);
        }

        // Update the inventory UI after sorting items
        inventoryUI.UpdateUI();
    }

    int GetSortCategory(Item item)
    {
        if (item is Equipment equip && equip.equipmentType == EquipmentType.Weapon) return 0;
        if (item is Equipment) return 1;
        if (item is Consumable) return 2;
        if (item is Collectable) return 3;
        return 4;
    }

    public int GetFreeSlotCount()
    {
        int free = 0;
        foreach (InventorySlotData slot in items)
        {
            if (slot == null)
                free++;
        }
        return free;
    }

    public void RemoveItemByID(string itemID, int quantityToRemove)
    {
        // Validate the quantity to remove
        int remainingToRemove = quantityToRemove;

        // Iterate through the inventory slots to find and remove the specified quantity of the item
        for (int i = 0; i < items.Length && remainingToRemove > 0; i++)
        {
            // Check if the current slot is not null and contains the item to be removed
            if (items[i] != null && items[i].item.ITEM_ID == itemID)
            {
                // Determine how much to remove from this slot
                int removedFromThisSlot = 0;

                // If the quantity in this slot is less than or equal to the remaining quantity to remove,
                // remove the entire stack
                if (items[i].quantity <= remainingToRemove)
                {
                    // Remove the entire stack from this slot
                    removedFromThisSlot = items[i].quantity;

                    // Update the remaining quantity to remove
                    remainingToRemove -= items[i].quantity;

                    // Clear the inventory slot by setting it to null
                    items[i] = null;

                    // save the updated inventory state for the cleared slot`
                    Save.SaveInventory(null, i);
                }
                else
                {
                    // If the quantity in this slot is greater than the remaining quantity to remove,
                    removedFromThisSlot = remainingToRemove;

                    // Decrease the quantity in this slot by the remaining quantity to remove
                    items[i].quantity -= remainingToRemove;

                    // save the updated inventory state for the slot after decreasing the quantity
                    Save.SaveInventory(items[i], i);

                    // Set remainingToRemove to zero since we've removed the required amount
                    remainingToRemove = 0;
                }
            }
        }

        // Update the inventory UI after removing the item(s)
        inventoryUI.UpdateUI();

        // Notify the quest system about the item removal
        NotifyQuestSystemItemRemoved(itemID, quantityToRemove);
    }

    public void RemoveItemBySlot(int slotIndex, int quantityToRemove = -1)
    {
        // Validate the slot index and check if the slot is not null
        if (slotIndex < 0 || slotIndex >= items.Length || items[slotIndex] == null) return;

        // Get the item ID of the item in the specified slot
        string itemID = items[slotIndex].item.ITEM_ID;

        // Determine the actual quantity removed based on the specified quantity to remove
        int actualQuantityRemoved;

        // If quantityToRemove is -1 or greater than or equal to the quantity in the slot, remove the entire stack
        if (quantityToRemove == -1 || quantityToRemove >= items[slotIndex].quantity)
        {
            // Remove the entire stack from the specified slot
            actualQuantityRemoved = items[slotIndex].quantity;

            // Clear the inventory slot by setting it to null
            items[slotIndex] = null;

            // save the updated inventory state for the cleared slot
            Save.SaveInventory(null, slotIndex);
        }
        else
        {
            // If quantityToRemove is less than the quantity in the slot, decrease the quantity in the slot
            actualQuantityRemoved = quantityToRemove;

            // Decrease the quantity in the specified slot by the specified amount
            items[slotIndex].quantity -= quantityToRemove;

            // save the updated inventory state for the slot after decreasing the quantity
            Save.SaveInventory(items[slotIndex], slotIndex);
        }

        // Update the inventory UI after removing the item(s)
        inventoryUI.UpdateUI();

        // Notify the quest system about the item removal with the actual quantity removed
        NotifyQuestSystemItemRemoved(itemID, actualQuantityRemoved);
    }

    void NotifyQuestSystemItemRemoved(string itemID, int quantity)
    {
        if (playerquests != null)
        {
            playerquests.OnItemRemoved(itemID, quantity);
        }
    }

    public void CoinCollected(float amount)
    {
        Stats.Coins += amount;
        inventoryUI.CoinText.text = $"{Stats.Coins}<sprite index=0>";

        Save.SaveStats();
        OnCoinsChanged?.Invoke();
    }

    public void CoinSpent(float amount)
    {
        Stats.Coins -= amount;
        inventoryUI.CoinText.text = $"{Stats.Coins}<sprite index=0>";

        Save.SaveStats();
        OnCoinsChanged?.Invoke();
    }

    void OnLevelUp()
    {
        inventoryUI.UpdateUI();
    }

    public bool HasEquipmentUpgrade()
    {
        foreach (InventorySlotData slot in items)
        {
            if (slot?.item is Equipment equip)
            {
                if (Stats.PlayerLevel.Value < equip.LevelRequirement) continue;

                Equipment equipped = equipmentManager.currentEquipment[(int)equip.equipmentType]?.item as Equipment;
                if (equipped == null || equip.LevelRequirement > equipped.LevelRequirement)
                    return true;
            }
        }
        return false;
    }
}
