using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] UpgradeUI upgradeUI;
    public InventorySlotData upgradeSlotData;
    public int coinCost;
    public int collectableCost;

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

        // Set UI
        upgradeUI.AssignIcon(fromSlot.slotData);
        upgradeUI.AssignName(fromSlot.slotData);
        upgradeUI.AssignStats(fromSlot.slotData);
        upgradeUI.AssignData(fromSlot.slotData);
        upgradeUI.AssignCost(fromSlot.slotData);
    }

    public void ClearSlot()
    {
        upgradeSlotData = null;
    }

    public void CalculateCost(InventorySlotData slotData)
    {
        // calculate item cost based on the required level and Quality
        Equipment equipment = (Equipment)slotData.item;

        switch (equipment.LevelRequirement)
        {
            case 1:
                switch (equipment.ItemQuality)
                {
                    case ItemQuality.Normal:
                        coinCost = 5;
                        collectableCost = 3;
                        break;
                    case ItemQuality.Good:
                        coinCost = 6;
                        collectableCost = 4;
                        break;
                    case ItemQuality.Great:
                        coinCost = 7;
                        collectableCost = 5;
                        break;
                    case ItemQuality.Excellent:
                        coinCost = 8;
                        collectableCost = 6;
                        break;
                }
                break;

            case 5:
                switch (equipment.ItemQuality)
                {
                    case ItemQuality.Normal:
                        coinCost = 9;
                        collectableCost = 7;
                        break;
                    case ItemQuality.Good:
                        coinCost = 10;
                        collectableCost = 8;
                        break;
                    case ItemQuality.Great:
                        coinCost = 11;
                        collectableCost = 9;
                        break;
                    case ItemQuality.Excellent:
                        coinCost = 12;
                        collectableCost = 10;
                        break;
                }
                break;

            case 10:
                switch (equipment.ItemQuality)
                {
                    case ItemQuality.Normal:
                        coinCost = 13;
                        collectableCost = 11;
                        break;
                    case ItemQuality.Good:
                        coinCost = 14;
                        collectableCost = 12;
                        break;
                    case ItemQuality.Great:
                        coinCost = 15;
                        collectableCost = 13;
                        break;
                    case ItemQuality.Excellent:
                        coinCost = 16;
                        collectableCost = 14;
                        break;
                }
                break;
        }
    }
}
