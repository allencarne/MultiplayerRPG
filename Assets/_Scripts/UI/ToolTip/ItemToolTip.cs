using UnityEngine;
using UnityEngine.EventSystems;

public class ItemToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler, ICancelHandler
{
    [SerializeField] Player player;
    [SerializeField] InventorySlot inventorySlot;
    [SerializeField] EquipmentSlot equipment;

    [Header("Context Menu")]
    [SerializeField] GameObject contextMenu;
    [SerializeField] GameObject button_Use;
    [SerializeField] GameObject button_Split;
    [SerializeField] GameObject button_Drop;
    [SerializeField] GameObject button_Sell;
    [SerializeField] GameObject button_Upgrade;

    private void OnDisable()
    {
        player.HideToolTip();
    }

    private InventorySlotData GetCurrentItem()
    {
        // Check if inventory slot has an item
        if (inventorySlot != null && inventorySlot.slotData != null && inventorySlot.slotData.item != null)
        {
            // Return the item from the inventory slot
            return inventorySlot.slotData;
        }

        // Check if equipment slot has an item
        if (equipment != null && equipment.SlotData != null && equipment.SlotData.item != null)
        {
            // Return the item from the equipment slot
            return equipment.SlotData;
        }

        // If no item is found, return null
        return null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventorySlotData item = GetCurrentItem();
        if (item == null) return;

        // Show the tooltip for the selected item
        player.ShowToolTip(item);

        // Hide context menu if it is active
        if (contextMenu != null && contextMenu.activeSelf) player.HideToolTip();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        player.HideToolTip();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        player.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Left and Right Click Disables Tooltip
        player.HideToolTip();

        // If no inventory slot is assigned, return
        if (inventorySlot == null) return;

        // Left Click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Hide context menu if left click and no item is present
            if (inventorySlot.slotData == null) contextMenu.SetActive(false);
        }

        // Right Click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // If no context menu or no item is present, return
            if (contextMenu == null || inventorySlot.slotData == null) return;

            if (contextMenu.activeSelf)
            {
                // Hide context menu if already active
                contextMenu.SetActive(false);
            }
            else
            {
                // Show context menu if not active
                UpdateContextMenuButtons(inventorySlot.slotData.item);
                contextMenu.SetActive(true);
            }
        }
    }

    private void UpdateContextMenuButtons(Item item)
    {
        if (item == null || contextMenu == null) return;

        // Use is only available for Consumable, Equipment, Weapon
        bool canUse = item is Consumable || item is Equipment || item is Weapon;
        button_Use.SetActive(canUse);

        // Split is available for stackable Collectables and Consumables
        bool canSplit = item.IsStackable && !(item is Equipment);
        button_Split.SetActive(canSplit);

        // Sell
        bool canSell = player.CanSellItems;
        button_Sell.SetActive(canSell);

        // Upgrade
        bool isUpgradeable = item.ItemCategory == ItemCategory.Equipment || item.ItemCategory == ItemCategory.Weapon;
        bool canUpgrade = player.CanUpgradeItems;
        button_Upgrade.SetActive(isUpgradeable && canUpgrade);

        // Always available
        button_Drop.SetActive(true);
    }

    public void OnCancel(BaseEventData eventData)
    {
        // Hide ToolTip
        player.HideToolTip();

        // If no inventory slot or context menu is assigned, return
        if (inventorySlot == null) return;
        if (contextMenu == null) return;

        // Toggle context menu visibility
        if (contextMenu.activeSelf)
        {
            // Hide context menu if already active
            contextMenu.SetActive(false);
        }
        else
        {
            // Show context menu if not active
            UpdateContextMenuButtons(inventorySlot.slotData.item);
            contextMenu.SetActive(true);
            SelectFirstActiveButton();
        }
    }

    void SelectFirstActiveButton()
    {
        // Check buttons in priority order
        if (button_Use.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(button_Use);
        }
        else if (button_Split.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(button_Split);
        }
        else if (button_Drop.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(button_Drop);
        }
        else if (button_Sell.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(button_Sell);
        }
        else if (button_Upgrade.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(button_Upgrade);
        }
    }
}