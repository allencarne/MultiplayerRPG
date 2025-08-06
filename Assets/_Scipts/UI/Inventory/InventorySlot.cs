using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
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

    public void OnDrop(PointerEventData eventData)
    {
        // Get drag source
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        InventorySlot toSlot = this;

        // Don't allow dropping onto self
        if (fromSlot == toSlot) return;

        // Get their slot data
        InventorySlotData fromData = fromSlot.slotData;
        InventorySlotData toData = toSlot.slotData;

        // Swap logic
        fromSlot.slotData = toData;
        toSlot.slotData = fromData;

        // Update Inventory
        fromSlot.inventory.items[fromSlot.slotIndex] = fromSlot.slotData;
        toSlot.inventory.items[toSlot.slotIndex] = toSlot.slotData;

        // Update visuals
        fromSlot.UpdateSlotVisuals();
        toSlot.UpdateSlotVisuals();

        // Save
        fromSlot.inventory.initialize.SaveInventory(fromSlot.slotData?.item, fromSlot.slotIndex, fromSlot.slotData?.quantity ?? 0);
        toSlot.inventory.initialize.SaveInventory(toSlot.slotData?.item, toSlot.slotIndex, toSlot.slotData?.quantity ?? 0);
    }

    public void UpdateSlotVisuals()
    {
        if (slotData != null)
        {
            icon.sprite = slotData.item.Icon;
            icon.color = Color.white;
            amountText.text = (slotData.quantity > 1) ? slotData.quantity.ToString() : "";
        }
        else
        {
            icon.sprite = null;
            icon.color = Color.black;
            amountText.text = "";
        }
    }
}