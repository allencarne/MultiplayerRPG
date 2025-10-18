using System;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private ItemList itemDatabase;
    [SerializeField] PlayerInitialize initialize;

    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentUI equipmentUI;
    [SerializeField] PlayerEquipment equipment;
    public Equipment[] currentEquipment;

    private void Awake()
    {
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;
        currentEquipment = new Equipment[numberOfSlots];
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipmentType;

        Equipment oldItem = null;

        // Remove the newItem from inventory
        int itemIndex = Array.FindIndex(inventory.items, slot => slot != null && slot.item == newItem);

        if (itemIndex != -1)
        {
            inventory.items[itemIndex] = null;
            initialize.SaveInventory(null, itemIndex, 0);
        }

        // If there is already a piece of equipment in the slot
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.AddItem(oldItem, 1 ,true); // move old item back to inventory
        }

        equipmentUI.UpdateUI(newItem, oldItem);
        equipment.OnEquipmentChanged(newItem, oldItem);

        currentEquipment[slotIndex] = newItem;

        // Save new equipment
        initialize.SaveEquipment(newItem, slotIndex);
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
            initialize.SaveEquipment(null, slotIndex);
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
        string prefix = initialize.CharacterNumber;

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
                    equipment.OnEquipmentChanged(newItem, oldItem, false);
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
    }
}
