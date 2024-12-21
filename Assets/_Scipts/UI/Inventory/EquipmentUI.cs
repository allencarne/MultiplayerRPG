using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] EquipmentManager equipmentManager;
    public Transform equipmentItemsParent;
    EquipmentSlot[] equipmentSlots;

    private void Start()
    {
        equipmentSlots = equipmentItemsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    public void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        // Determine the slot index based on the equipment type of newItem or oldItem
        int slotIndex = (newItem != null) ? (int)newItem.equipmentType : (int)oldItem.equipmentType;

        // Check if the slot index is valid
        if (slotIndex >= 0 && slotIndex < equipmentSlots.Length)
        {
            // If newItem is not null, add it to the equipment slot; otherwise, clear the slot
            if (newItem != null)
            {
                equipmentSlots[slotIndex].AddItem(newItem);
            }
            else
            {
                equipmentSlots[slotIndex].ClearSlot();
            }
        }
        else
        {
            Debug.LogError("Invalid slot index in EquipmentUI.UpdateUI.");
        }
    }
}
