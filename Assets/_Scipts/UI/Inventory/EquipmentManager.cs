using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentUI equipmentUI;
    [SerializeField] PlayerEquipment equipment;
    public Equipment[] currentEquipment;

    private void Start()
    {
        // Get the number of equipment slots
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;

        // Define Array Length
        currentEquipment = new Equipment[numberOfSlots];
    }

    public void Equip(Equipment newItem)
    {
        // Gets the index that the new item is supposed to be slotted into
        int slotIndex = (int)newItem.equipmentType;

        Equipment oldItem = null;

        // If there is already a piece of equipment in the slot
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.AddItem(oldItem);
        }

        equipmentUI.UpdateUI(newItem, oldItem);
        equipment.OnEquipmentChanged(newItem, oldItem);

        // Set new item
        currentEquipment[slotIndex] = newItem;
    }

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.AddItem(oldItem);

            currentEquipment[slotIndex] = null;

            equipmentUI.UpdateUI(null, oldItem);
            equipment.OnEquipmentChanged(null, oldItem);
        }
    }

    public void UnequipAll()
    {

    }
}
