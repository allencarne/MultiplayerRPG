using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Inventory inventory;
    public EquipmentManager equipmentManager;
    public InventoryItem inventoryItem;
    public int slotIndex;
    public Image icon;
    public TextMeshProUGUI amountText;

    public void AddItem(Item newItem)
    {
        inventoryItem.Item = newItem;

        inventory.items[slotIndex] = newItem;
        icon.sprite = newItem.Icon;
        icon.color = Color.white;

        // Stack Text
        if (amountText != null && newItem.Quantity > 1)
        {
            amountText.text = newItem.Quantity.ToString();
        }
        else
        {
            amountText.text = "";
        }
    }

    public void UseItem()
    {
        if (inventoryItem.Item != null)
        {
            inventoryItem.Item.Use(inventory,equipmentManager);
        }
    }

    public void ClearSlot()
    {
        inventoryItem.Item = null;

        inventory.items[slotIndex] = null; // Update the inventory array
        icon.sprite = null;
        icon.enabled = true; // Ensure the Image component is always enabled
        icon.color = Color.black;

        // Clear Stacks
        if (amountText != null)
        {
            amountText.text = "";
        }
    }

    public void RemoveItem()
    {
        inventory.RemoveItem(inventoryItem.Item);
    }
}
