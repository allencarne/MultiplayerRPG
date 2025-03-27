using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public Transform equipmentItemsParent;
    private Dictionary<EquipmentType, EquipmentSlot> equipmentSlots;

    private void Start()
    {
        EquipmentSlot[] slots = equipmentItemsParent.GetComponentsInChildren<EquipmentSlot>();
        equipmentSlots = new Dictionary<EquipmentType, EquipmentSlot>();

        foreach (var slot in slots)
        {
            equipmentSlots[(EquipmentType)slot.index] = slot;
        }
    }

    public void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        EquipmentType equipmentType = (newItem != null) ? newItem.equipmentType : oldItem.equipmentType;

        if (equipmentSlots.TryGetValue(equipmentType, out EquipmentSlot slot))
        {
            if (newItem != null)
            {
                slot.AddItem(newItem);
            }
            else
            {
                slot.ClearSlot();
            }
        }
        else
        {
            Debug.LogError($"No EquipmentSlot found for {equipmentType}.");
        }
    }
}
