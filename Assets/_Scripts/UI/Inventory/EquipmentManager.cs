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
    public Equipment[] currentEquipment;

    private void Awake()
    {
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numberOfSlots];
    }

    public bool Equip(Equipment newItem)
    {
        if (stats.PlayerLevel.Value < newItem.LevelRequirement)
        {
            Debug.Log($"Level {newItem.LevelRequirement} required to equip {newItem.name}.");
            return false;
        }

        int slotIndex = (int)newItem.equipmentType;
        Equipment oldItem = null;

        // Remove the newItem from inventory
        int itemIndex = Array.FindIndex(inventory.items, slot => slot != null && slot.item == newItem);
        if (itemIndex != -1)
        {
            inventory.items[itemIndex] = null;
            save.SaveInventory(null, itemIndex, 0);
        }

        // If there is already a piece of equipment in the slot
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.AddItem(oldItem, 1 ,true);
        }

        equipmentUI.UpdateUI(newItem, oldItem);
        equipment.OnEquipmentChanged(newItem, oldItem);
        currentEquipment[slotIndex] = newItem;
        save.SaveEquipment(newItem, slotIndex);
        inventory.inventoryUI.UpdateUI();

        return true;
    }

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            bool added = inventory.AddItem(oldItem, 1 ,true);

            if (!added)
            {
                Debug.Log($"Inventory full — could not unequip {oldItem.name}.");
                return;
            }

            currentEquipment[slotIndex] = null;
            equipmentUI.UpdateUI(null, oldItem);
            equipment.OnEquipmentChanged(null, oldItem);
            save.SaveEquipment(null, slotIndex);

            // Refresh UI
            inventory.inventoryUI.UpdateUI();
        }
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
                    Equipment newItem = Instantiate(equipmentTemplate);
                    Equipment oldItem = currentEquipment[slotIndex];

                    currentEquipment[slotIndex] = newItem;
                    equipmentUI.UpdateUI(newItem, oldItem);
                    equipment.OnEquipmentChanged(newItem, oldItem, true);
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
