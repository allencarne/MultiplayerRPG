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
        // Get the number of equipment slots
        int numberOfSlots = System.Enum.GetNames(typeof(EquipmentType)).Length;

        // Define Array Length
        currentEquipment = new Equipment[numberOfSlots];
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipmentType;

        Equipment oldItem = null;

        // Remove the newItem from inventory
        int itemIndex = System.Array.IndexOf(inventory.items, newItem);
        if (itemIndex != -1)
        {
            inventory.items[itemIndex] = null;
            initialize.SaveInventory(null, itemIndex);
        }

        // If there is already a piece of equipment in the slot
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.AddItem(oldItem); // move old item back to inventory
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
            inventory.AddItem(oldItem);

            currentEquipment[slotIndex] = null;

            equipmentUI.UpdateUI(null, oldItem);
            equipment.OnEquipmentChanged(null, oldItem);

            // Clear equipment slot in save
            initialize.SaveEquipment(null, slotIndex);
        }
    }

    public void UnequipAll()
    {

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
                string saved = PlayerPrefs.GetString(key);
                string[] parts = saved.Split('|');

                if (parts.Length == 2)
                {
                    string itemName = parts[0];
                    int quantity = int.Parse(parts[1]);

                    Item baseItem = itemDatabase.GetItemByName(itemName);
                    if (baseItem is Equipment equipmentTemplate)
                    {
                        Equipment newItem = Instantiate(equipmentTemplate);
                        newItem.Quantity = quantity;

                        Equipment oldItem = currentEquipment[slotIndex];

                        currentEquipment[slotIndex] = newItem;
                        equipmentUI.UpdateUI(newItem, oldItem);
                        equipment.OnEquipmentChanged(newItem, oldItem);
                    }
                    else
                    {
                        Debug.LogWarning($"Item '{itemName}' is not a valid Equipment.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Malformed equipment string for key: {key}");
                }
            }
            else
            {
                currentEquipment[slotIndex] = null;
            }
        }
    }
}
