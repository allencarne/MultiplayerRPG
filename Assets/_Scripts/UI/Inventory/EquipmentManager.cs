using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerSave save;
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentUI equipmentUI;
    [SerializeField] PlayerEquipment equipment;

    [SerializeField] private ItemList itemDatabase;
    public InventorySlotData[] currentEquipment;

    private void Awake()
    {
        int numberOfSlots = Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new InventorySlotData[numberOfSlots];
    }

    public bool Equip(InventorySlotData newSlot)
    {
        Equipment newItem = newSlot.item as Equipment;
        if (newItem == null) return false;

        if (stats.PlayerLevel.Value < newItem.LevelRequirement)
        {
            Debug.Log($"Level {newItem.LevelRequirement} required to equip {newItem.name}.");
            return false;
        }

        if (stats.playerClass.ToString() != newItem.ClassRequirement.ToString())
        {
            if (newItem.ClassRequirement != ClassRequirement.None)
            {
                Debug.Log($"Class {newItem.ClassRequirement} required to equip {newItem.name}.");
                return false;
            }
        }

        int slotIndex = (int)newItem.equipmentType;
        InventorySlotData oldSlot = currentEquipment[slotIndex];

        int itemIndex = Array.IndexOf(inventory.items, newSlot);
        if (itemIndex != -1)
        {
            inventory.items[itemIndex] = null;
            save.SaveInventory(null, itemIndex);
        }

        if (oldSlot != null)
        {
            inventory.AddItem(oldSlot, true);
        }

        equipmentUI.UpdateUI(newSlot, oldSlot);
        equipment.OnEquipmentChanged(newSlot, oldSlot);
        currentEquipment[slotIndex] = newSlot;
        save.SaveEquipment(newSlot, slotIndex);
        inventory.inventoryUI.UpdateUI();

        return true;
    }

    public void UnEquip(int slotIndex)
    {
        InventorySlotData oldSlot = currentEquipment[slotIndex];
        if (oldSlot == null) return;

        bool added = inventory.AddItem(oldSlot, true);
        if (!added)
        {
            Debug.Log($"Inventory full — could not unequip {oldSlot.item.name}.");
            return;
        }

        currentEquipment[slotIndex] = null;
        equipmentUI.UpdateUI(null, oldSlot);
        equipment.OnEquipmentChanged(null, oldSlot);
        save.SaveEquipment(null, slotIndex);
        inventory.inventoryUI.UpdateUI();
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            UnEquip(i);
        }
    }

    public void LoadEquipment()
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

        // Loop through all equipment slot indices (0–7)
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;

        for (int slotIndex = 0; slotIndex < numberOfSlots; slotIndex++)
        {
            string key = $"{prefix}EquipmentSlot_{slotIndex}";

            if (PlayerPrefs.HasKey(key))
            {
                string saved = PlayerPrefs.GetString(key);
                string[] parts = saved.Split('|');

                if (parts.Length >= 1 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    string itemName = parts[0];
                    Item baseItem = itemDatabase.GetItemByName(itemName);

                    if (baseItem is Equipment equipmentTemplate)
                    {
                        // default to template values
                        ItemRarity rarity = equipmentTemplate.ItemRarity;
                        ItemQuality quality = equipmentTemplate.ItemQuality;
                        List<StatModifier> modifiers = new List<StatModifier>(equipmentTemplate.modifiers ?? new List<StatModifier>());

                        // Parse stored rarity/quality/modifiers if present
                        if (parts.Length >= 3)
                        {
                            // parts[1] => rarity, parts[2] => quality
                            if (!Enum.TryParse(parts[1], out rarity))
                            {
                                if (int.TryParse(parts[1], out int rInt)) rarity = (ItemRarity)rInt;
                            }
                            if (!Enum.TryParse(parts[2], out quality))
                            {
                                if (int.TryParse(parts[2], out int qInt)) quality = (ItemQuality)qInt;
                            }
                        }

                        if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3]))
                        {
                            string modsPart = parts[3];
                            string[] modEntries = modsPart.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            modifiers = new List<StatModifier>();
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

                        InventorySlotData newSlot = new InventorySlotData(equipmentTemplate, 1, rarity, quality, modifiers);

                        InventorySlotData oldSlot = currentEquipment[slotIndex];
                        currentEquipment[slotIndex] = newSlot;
                        equipmentUI.UpdateUI(newSlot, oldSlot);
                        equipment.OnEquipmentChanged(newSlot, oldSlot, true);
                    }
                    else
                    {
                        Debug.LogWarning($"Item '{itemName}' is not a valid Equipment.");
                    }
                }
                else
                {
                    currentEquipment[slotIndex] = null;
                }
            }
            else
            {
                currentEquipment[slotIndex] = null;
            }
        }

        inventory.inventoryUI.UpdateUI();
    }
}
