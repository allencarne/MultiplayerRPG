using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public Transform equipmentItemsParent;
    private Dictionary<EquipmentType, EquipmentSlot> equipmentSlots;

    private void Awake()
    {
        EquipmentSlot[] slots = equipmentItemsParent.GetComponentsInChildren<EquipmentSlot>();
        equipmentSlots = new Dictionary<EquipmentType, EquipmentSlot>();

        foreach (var slot in slots)
        {
            equipmentSlots[(EquipmentType)slot.index] = slot;
        }
    }

    public void UpdateUI(InventorySlotData newSlot, InventorySlotData oldSlot)
    {
        Equipment newItem = newSlot?.item as Equipment;
        Equipment oldItem = oldSlot?.item as Equipment;
        EquipmentType equipmentType = (newItem != null) ? newItem.equipmentType : oldItem.equipmentType;

        if (equipmentSlots.TryGetValue(equipmentType, out EquipmentSlot slot))
        {
            if (newSlot != null) slot.AddItem(newSlot);
            else slot.ClearSlot();
        }
        else
        {
            Debug.LogError($"No EquipmentSlot found for {equipmentType}.");
        }
    }
}
