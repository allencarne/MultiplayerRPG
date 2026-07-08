using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public EquipmentManager equipmentManager;
    [HideInInspector] public Inventory inventory;
    [SerializeField] ItemDrag itemDrag;
    public InventorySlotData slotData;

    public int slotIndex;
    public Image itemIcon;
    public Image itemBackground;
    public Image redTint;
    public TextMeshProUGUI amountText;

    [SerializeField] Color defaultColor;
    [SerializeField] Color upgradeColor;
    [SerializeField] Color downgradeColor;
    [SerializeField] Color SameLevelColor;

    public void AddItem(InventorySlotData data)
    {
        // Store the runtime data for this inventory slot
        // This contains the item, quantity, rarity, quality, etc
        slotData = data;

        // Display the item's icon
        itemIcon.sprite = data.item.Icon;

        // Make sure the icon is visible
        itemIcon.enabled = true;

        // Set the background color based on the item's rarity
        itemBackground.color = data.item.GetRarityColor(data.rarity);

        // Show a red tint if the player is too low level to use this item
        redTint.enabled = IsUnderLevelRequirement(data.item);

        // Update the text shown in the bottom corner of the slot.
        // (Level requirement for equipment, stack count for consumables.)
        RefreshAmountText(data.item, data.quantity);
    }

    public void UseItem()
    {
        // If the player was dragging this item, don't use it.
        // This prevents releasing a drag on the same slot from counting as a click
        if (itemDrag.canDrag) return;

        // Make sure this slot actually contains an item before trying to use it
        if (slotData?.item != null)
        {
            // Use the Item
            slotData.item.Use(inventory, equipmentManager, slotData);
        }
    }

    public void ClearSlot()
    {
        // Remove the runtime data stored in this slot
        slotData = null;

        // Clear the matching slot in the inventory data
        inventory.items[slotIndex] = null;

        // Remove the displayed icon
        itemIcon.sprite = null;

        // Hide the icon image
        itemIcon.enabled = false;

        // Restore the empty slot background color
        itemBackground.color = defaultColor;

        // Hide the red tint
        redTint.enabled = false;

        // Clear the amount/level text
        ClearStacks();
    }

    public void RemoveItem()
    {
        // Only attempt to remove something if an item exists
        if (slotData?.item != null)
        {
            // Remove the item from the inventory using this slot's index
            inventory.RemoveItemBySlot(slotIndex);
        }
    }

    void ClearStacks()
    {
        // Make sure the text component exists
        if (amountText != null)
        {
            // Reset the text color
            amountText.color = Color.white;

            // Remove any displayed number
            amountText.text = "";
        }
    }

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

        // Handle moving the item into this slot
        HandleDropFrom(fromSlot);
    }

    public void HandleDropFrom(InventorySlot fromSlot)
    {
        // Ignore invalid drops or dropping onto the same slot
        if (fromSlot == null || fromSlot == this) return;

        // "this" slot is the destination
        InventorySlot toSlot = this;

        // Cache both slot's data for readability
        InventorySlotData fromData = fromSlot.slotData;
        InventorySlotData toData = toSlot.slotData;

        // If both slots contain the same stackable item with the same rarity and quality
        if (toData != null && fromData != null && fromData.item.IsStackable && fromData.item.name == toData.item.name && fromData.rarity == toData.rarity && fromData.quality == toData.quality)
        {
            // Combine the stack quantities
            toData.quantity += fromData.quantity;

            // Clear the original slot
            fromSlot.slotData = null;
            fromSlot.inventory.items[fromSlot.slotIndex] = null;

            // Save both inventory slots
            fromSlot.inventory.Save.SaveInventory(null, fromSlot.slotIndex);
            toSlot.inventory.Save.SaveInventory(toData, toSlot.slotIndex);

            // Refresh both slot visuals
            fromSlot.UpdateSlotVisuals();
            toSlot.UpdateSlotVisuals();

            return;
        }

        // Otherwise swap the contents of the two slots
        fromSlot.slotData = toData;
        toSlot.slotData = fromData;

        // Keep the inventory array synchronized with the UI
        fromSlot.inventory.items[fromSlot.slotIndex] = fromSlot.slotData;
        toSlot.inventory.items[toSlot.slotIndex] = toSlot.slotData;

        // Refresh both slots so they display their new contents
        fromSlot.UpdateSlotVisuals();
        toSlot.UpdateSlotVisuals();

        // Save the new slot contents
        fromSlot.inventory.Save.SaveInventory(fromSlot.slotData, fromSlot.slotIndex);
        toSlot.inventory.Save.SaveInventory(toSlot.slotData, toSlot.slotIndex);
    }

    public void UpdateSlotVisuals()
    {
        // Does this slot currently contain an item?
        if (slotData != null)
        {
            // Display the correct icon
            itemIcon.sprite = slotData.item.Icon;

            // Ensure the icon is visible
            itemIcon.enabled = true;

            // Set Item Background color based on rarity
            itemBackground.color = slotData.item.GetRarityColor(slotData.rarity);

            // Show or hide the red tint depending on the player's level
            redTint.enabled = IsUnderLevelRequirement(slotData.item);

            // Update the amount/level text
            RefreshAmountText(slotData.item, slotData.quantity);
        }
        else
        {
            // Empty slot, so remove the icon
            itemIcon.sprite = null;

            // Hide the icon image
            itemIcon.enabled = false;

            // Restore the default empty background
            itemBackground.color = defaultColor;

            // Clear the displayed text
            amountText.text = "";

            // Restore the default text color
            amountText.color = Color.white;

            // Hide the level restriction tint
            redTint.enabled = false;
        }
    }

    bool IsUnderLevelRequirement(Item item)
    {
        return item is Equipment equip && inventory.Stats.PlayerLevel.Value < equip.LevelRequirement;
    }

    void RefreshAmountText(Item item, int quantity)
    {
        // Equipment displays its required level
        if (item is Equipment equip)
        {
            // Set Amount Text
            amountText.text = equip.LevelRequirement.ToString();

            // Color the level depending on whether it's an upgrade.
            amountText.color = GetEquipmentComparisonColor(equip);
        }
        else
        {
            // Stackable items display their quantity
            // Hide the number if there's only one
            amountText.text = (quantity > 1) ? quantity.ToString() : "";

            // Normal stack counts are always white
            amountText.color = Color.white;
        }
    }

    Color GetEquipmentComparisonColor(Equipment equip)
    {
        // Get the currently equipped item for this equipment slot
        Equipment equipped = equipmentManager.currentEquipment[(int)equip.equipmentType]?.item as Equipment;

        // Nothing equipped means this item is automatically an upgrade
        if (equipped == null) return upgradeColor;

        // Higher level than equipped
        if (equip.LevelRequirement > equipped.LevelRequirement) return upgradeColor;

        // Lower level than equipped
        if (equip.LevelRequirement < equipped.LevelRequirement) return downgradeColor;

        // Same level as equipped
        return SameLevelColor;
    }
}