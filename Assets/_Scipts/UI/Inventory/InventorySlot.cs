using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Inventory inventory;
    public InventoryItem inventoryItem;
    public int slotIndex;
    public Image icon;

    public void AddItem(Item newItem)
    {
        inventoryItem.item = newItem;

        inventory.items[slotIndex] = newItem;
        icon.sprite = newItem.icon;
        icon.color = Color.white;
    }

    public void ClearSlot()
    {

    }

    public void UseItem()
    {

    }

    public void RemoveItem()
    {
        inventory.RemoveItem(inventoryItem.item);
    }
}
