using System;
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
        save.SaveEquipment(newItem, slotIndex);
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
                string itemName = PlayerPrefs.GetString(key);
                Item baseItem = itemDatabase.GetItemByName(itemName);

                if (baseItem is Equipment equipmentTemplate)
                {
                    InventorySlotData newSlot = new InventorySlotData(equipmentTemplate, 1, equipmentTemplate.ItemRarity, equipmentTemplate.ItemQuality, equipmentTemplate.modifiers);

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

        inventory.inventoryUI.UpdateUI();
    }
}
