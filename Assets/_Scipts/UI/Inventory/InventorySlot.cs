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

    public void AddItem(Item newItem, int quantity)
    {
        inventoryItem.Item = newItem;

        inventory.items[slotIndex] = new InventorySlotData(newItem, quantity);
        icon.sprite = newItem.Icon;
        icon.color = Color.white;

        amountText.text = (quantity > 1) ? quantity.ToString() : "";
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

        inventory.items[slotIndex] = null;
        icon.sprite = null;
        icon.enabled = true;
        icon.color = Color.black;

        ClearStacks();
    }

    public void RemoveItem()
    {
        inventory.RemoveItem(inventoryItem.Item);
    }

    void ClearStacks()
    {
        if (amountText != null)
        {
            amountText.text = "";
        }
    }
}