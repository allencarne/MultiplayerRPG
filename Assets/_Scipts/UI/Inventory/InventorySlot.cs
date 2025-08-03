using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class InventorySlot : MonoBehaviour
{
    public EquipmentManager equipmentManager;
    public Inventory inventory;
    public InventorySlotData slotData;
    public int slotIndex;

    public Image icon;
    public TextMeshProUGUI amountText;

    public void AddItem(Item newItem, int quantity)
    {
        slotData = new InventorySlotData(newItem, quantity);
        inventory.items[slotIndex] = slotData;

        icon.sprite = newItem.Icon;
        icon.color = Color.white;

        amountText.text = (quantity > 1) ? quantity.ToString() : "";
    }

    public void UseItem()
    {
        if (slotData?.item != null)
        {
            slotData.item.Use(inventory, equipmentManager);
        }
    }


    public void ClearSlot()
    {
        slotData = null;
        inventory.items[slotIndex] = null;

        icon.sprite = null;
        icon.enabled = true;
        icon.color = Color.black;

        ClearStacks();
    }

    public void RemoveItem()
    {
        if (slotData?.item != null)
        {
            inventory.RemoveItem(slotData.item);
        }
    }

    void ClearStacks()
    {
        if (amountText != null)
        {
            amountText.text = "";
        }
    }
}