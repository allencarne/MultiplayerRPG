using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : MonoBehaviour, IDropHandler
{
    // Reference
    [SerializeField] Inventory inventory;
    [SerializeField] UpgradeUI upgradeUI;
    public Item[] collectables;

    [HideInInspector] public InventorySlotData upgradeSlotData;
    [HideInInspector] public Item currentCollectable;
    [HideInInspector] public int coinCost;
    [HideInInspector] public int collectableCost;

    int fromInventorySlotIndex = -1;
    int[] LevelBreakpoints = { 1, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };

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
        if (fromSlot.slotData.item.ItemCategory != ItemCategory.Equipment && fromSlot.slotData.item.ItemCategory != ItemCategory.Weapon) return;

        // Assign Upgrade Slot
        upgradeSlotData = fromSlot.slotData;
        fromInventorySlotIndex = fromSlot.slotIndex;

        // Set UI
        upgradeUI.AssignIcon(upgradeSlotData);
        upgradeUI.AssignName(upgradeSlotData);
        upgradeUI.AssignStats(upgradeSlotData);
        upgradeUI.AssignData(upgradeSlotData);
        upgradeUI.AssignCost(upgradeSlotData);
        upgradeUI.AssignButtons(upgradeSlotData);
        upgradeUI.RefreshUpgradeButtons();

        // Remove Equipment from Inventory while it's checked out for upgrading
        inventory.RemoveItemBySlot(fromInventorySlotIndex);
    }

    public void ClearSlot()
    {
        upgradeSlotData = null;
        currentCollectable = null;
        fromInventorySlotIndex = -1;
    }

    public void ReturnItemAndClear()
    {
        if (upgradeSlotData != null)
        {
            inventory.AddItem(upgradeSlotData);
        }

        ClearSlot();
    }

    public void CalculateCost(int level, ItemQuality quality)
    {
        int baseCoinCost = 5;
        int baseCollectableCost = 3;

        // Convert the level requirement into a "tier index"
        int tierIndex = Array.IndexOf(LevelBreakpoints, level);

        // Cast quality as an int
        int qualityIndex = (int)quality;

        // Combine tier + quality into ONE overall progression number
        int overallStep = (tierIndex * Enum.GetNames(typeof(ItemQuality)).Length) + qualityIndex;

        // Derive coinCost / collectableCost from overallStep with a formula instead of a literal number per case.
        coinCost = baseCoinCost + overallStep;
        collectableCost = baseCollectableCost + overallStep;

        // Figure out which collectable this tier uses
        int value = tierIndex / 2;
        currentCollectable = collectables[value];
    }
}
