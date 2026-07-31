using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    InventorySlotData upgradeSlotData;
    [SerializeField] UpgradeUI upgradeUI;

    public void OnDrop(PointerEventData eventData)
    {
        // Try to find the ItemDrag component from the object being dragged
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();

        // Nothing was being dragged
        if (draggedItem == null) return;

        // Ignore if dragging wasn't actually active
        if (!draggedItem.canDrag) return;

        // Get the slot the item came from
        InventorySlot fromSlot = draggedItem.inventorySlot;

        // Return if slot is empty
        if (fromSlot == null) return;

        // Return if not Equipment or Weapon
        if (fromSlot.slotData.item.ItemCategory != ItemCategory.Equipment || fromSlot.slotData.item.ItemCategory != ItemCategory.Weapon)

        // Assign Upgrade Slot
        upgradeSlotData = fromSlot.slotData;

        // Remove Equipment from Inventory


        upgradeUI.AssignIcon(fromSlot.slotData);
        upgradeUI.AssignData(fromSlot.slotData);
    }
}
