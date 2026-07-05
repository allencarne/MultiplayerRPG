using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    [SerializeField] ItemList itemDatabase;
    [SerializeField] PlayerQuest playerquests;
    [SerializeField] PlayerExperience playerExperience;
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

        // Save the inventory state
        Save.SaveInventory(newItem, emptySlotIndex, quantity);

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
        if (newItem.IsStackable)
        {
            int existingIndex = Array.FindIndex(items, x => x != null && x.item.name == newItem.name);
            if (existingIndex != -1)
            {
                items[existingIndex].quantity += quantity;
                inventoryUI.UpdateUI();
                OnItemAdded?.Invoke(newItem, quantity);
                Save.SaveInventory(newItem, existingIndex, items[existingIndex].quantity);
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

            Save.SaveInventory(removedItem, itemIndex, 1);
        }

        inventoryUI.UpdateUI();
    }

    public void LoadInventory()
    {
        // prefix for the PlayerPrefs keys based on the selected character
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

        // for each inventory slot, check if there's a saved item and load it
        for (int i = 0; i < inventorySlots; i++)
        {
            // Construct the PlayerPrefs key for this slot
            string key = $"{prefix}InventorySlot_{i}";

            // Check if there's a saved item for this slot
            if (PlayerPrefs.HasKey(key))
            {
                // Retrieve the saved string for this slot
                string saved = PlayerPrefs.GetString(key);

                // Split the saved string into parts: item name and quantity
                string[] parts = saved.Split('|');

                // Validate the parts and parse the quantity
                if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && int.TryParse(parts[1], out int quantity))
                {
                    // Retrieve the item name from the saved string
                    string itemName = parts[0];

                    // Get the item template from the item database using the item name
                    Item template = itemDatabase.GetItemByName(itemName);

                    // If the template is found, create a new InventorySlotData for this slot
                    if (template != null)
                    {
                        // Create a new InventorySlotData for this slot using the template and saved quantity
                        ItemRarity rarity = template.ItemRarity;
                        ItemQuality quality = template.ItemQuality;
                        List<StatModifier> modifiers = new List<StatModifier>();

                        // If the saved string contains rarity and quality, parse them
                        if (parts.Length >= 4)
                        {
                            // parse enums by name or numeric fallback
                            if (!Enum.TryParse(parts[2], out rarity))
                            {
                                if (int.TryParse(parts[2], out int rInt)) rarity = (ItemRarity)rInt;
                            }
                            if (!Enum.TryParse(parts[3], out quality))
                            {
                                if (int.TryParse(parts[3], out int qInt)) quality = (ItemQuality)qInt;
                            }
                        }

                        // If the saved string contains modifiers, parse them
                        if (parts.Length >= 5 && !string.IsNullOrEmpty(parts[4]))
                        {
                            string modsPart = parts[4];
                            string[] modEntries = modsPart.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            // Parse each modifier entry and create StatModifier instances
                            foreach (string me in modEntries)
                            {
                                string[] modParts = me.Split(',');
                                if (modParts.Length == 3 &&
                                    float.TryParse(modParts[0], out float val) &&
                                    int.TryParse(modParts[1], out int statInt) &&
                                    int.TryParse(modParts[2], out int srcInt))
                                {
                                    StatModifier m = new StatModifier
                                    {
                                        value = val,
                                        statType = (StatType)statInt,
                                        source = (ModSource)srcInt
                                    };
                                    modifiers.Add(m);
                                }
                            }
                        }

                        items[i] = new InventorySlotData(template, quantity, rarity, quality, modifiers);
                    }
                    else
                    {
                        // If the template is not found, log a warning and clear the slot
                        Debug.LogWarning($"Item '{itemName}' not found in ItemDatabase.");

                        // Force clear the slot if the item is invalid
                        items[i] = null;
                    }
                }
                else
                {
                    // If the saved string is malformed, log a warning and clear the slot
                    Debug.LogWarning($"Malformed inventory string for key: {key}");

                    // Force clear the slot if the saved string is invalid
                    items[i] = null;
                }
            }
            else
            {
                // If there's no saved item for this slot, ensure the slot is null
                items[i] = null;
            }
        }

        // Update the inventory UI after loading all items
        inventoryUI.UpdateUI();
    }

    public bool AddItemToSlot(Item item, int quantity, int targetSlot)
    {
        if (items[targetSlot] == null)
        {
            items[targetSlot] = new InventorySlotData(item, quantity, item.ItemRarity, item.ItemQuality);
            Save.SaveInventory(item, targetSlot, quantity);
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

                if (targetSlot.item.name == sourceSlot.item.name && targetSlot.rarity == sourceSlot.rarity && targetSlot.quality == sourceSlot.quality)
                {
                    // Combine quantity into source
                    sourceSlot.quantity += targetSlot.quantity;

                    // Clear target slot
                    items[j] = null;
                    Save.SaveInventory(null, j, 0); // Clear saved slot
                }
            }

            // Save updated source slot
            Save.SaveInventory(sourceSlot.item, i, sourceSlot.quantity);
        }

        inventoryUI.UpdateUI();
    }

    public void SortButton()
    {
        // Collect all non-null slots
        List<InventorySlotData> sorted = new List<InventorySlotData>();
        foreach (InventorySlotData slot in items)
        {
            if (slot != null) sorted.Add(slot);
        }

        sorted.Sort((a, b) =>
        {
            int categoryA = GetSortCategory(a.item);
            int categoryB = GetSortCategory(b.item);

            if (categoryA != categoryB)
                return categoryA.CompareTo(categoryB);

            // Within equipment/weapons, sort by level requirement descending
            if (a.item is Equipment equipA && b.item is Equipment equipB)
                return equipB.LevelRequirement.CompareTo(equipA.LevelRequirement);

            // Otherwise sort alphabetically
            return string.Compare(a.item.name, b.item.name, StringComparison.OrdinalIgnoreCase);
        });

        // Redistribute back into slots
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = i < sorted.Count ? sorted[i] : null;
            Save.SaveInventory(items[i]?.item, i, items[i]?.quantity ?? 0);
        }

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
        int remainingToRemove = quantityToRemove;

        for (int i = 0; i < items.Length && remainingToRemove > 0; i++)
        {
            if (items[i] != null && items[i].item.ITEM_ID == itemID)
            {
                int removedFromThisSlot = 0;

                if (items[i].quantity <= remainingToRemove)
                {
                    // Remove entire stack
                    removedFromThisSlot = items[i].quantity;
                    remainingToRemove -= items[i].quantity;
                    items[i] = null;
                    Save.SaveInventory(null, i, 0);
                }
                else
                {
                    // Reduce quantity
                    removedFromThisSlot = remainingToRemove;
                    items[i].quantity -= remainingToRemove;
                    Save.SaveInventory(items[i].item, i, items[i].quantity);
                    remainingToRemove = 0;
                }
            }
        }

        inventoryUI.UpdateUI();
        NotifyQuestSystemItemRemoved(itemID, quantityToRemove);
    }

    public void RemoveItemBySlot(int slotIndex, int quantityToRemove = -1)
    {
        if (slotIndex < 0 || slotIndex >= items.Length || items[slotIndex] == null) return;

        string itemID = items[slotIndex].item.ITEM_ID;
        int actualQuantityRemoved;

        if (quantityToRemove == -1 || quantityToRemove >= items[slotIndex].quantity)
        {
            // Remove entire stack
            actualQuantityRemoved = items[slotIndex].quantity;
            items[slotIndex] = null;
            Save.SaveInventory(null, slotIndex, 0);
        }
        else
        {
            // Remove partial quantity
            actualQuantityRemoved = quantityToRemove;
            items[slotIndex].quantity -= quantityToRemove;
            Save.SaveInventory(items[slotIndex].item, slotIndex, items[slotIndex].quantity);
        }

        inventoryUI.UpdateUI();
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
