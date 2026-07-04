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

    private void OnDisable()
    {
        //tooltip.SetActive(false);
        player.HideToolTip();
    }

    private InventorySlotData GetCurrentItem()
    {
        if (inventorySlot != null &&
            inventorySlot.slotData != null &&
            inventorySlot.slotData.item != null)
        {
            return inventorySlot.slotData;
        }

        if (equipment != null &&
            equipment.SlotData != null &&
            equipment.SlotData.item != null)
        {
            return equipment.SlotData;
        }

        return null;
    }

    public void OnSelect(BaseEventData eventData)
    {
        InventorySlotData item = GetCurrentItem();
        if (item == null) return;

        player.ShowToolTip(item);

        if (contextMenu != null && contextMenu.activeSelf)
        {
            //tooltip.SetActive(false);
            player.HideToolTip();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //tooltip.SetActive(false);
        player.HideToolTip();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //tooltip.SetActive(false);
        player.HideToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Left and Right Click Disables Tooltip
        //tooltip.SetActive(false);
        player.HideToolTip();

        // Equipment doesn't have an Inventory Slot
        if (inventorySlot == null) return;

        // Left Click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inventorySlot.slotData == null)
            {
                contextMenu.SetActive(false);
            }
        }

        // Right Click
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu == null || inventorySlot.slotData == null) return;

            if (contextMenu.activeSelf)
            {
                contextMenu.SetActive(false);
            }
            else
            {
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

        // Always available
        button_Drop.SetActive(true);

        bool canSell = player.CanSellItems;
        button_Sell.SetActive(canSell);
    }

    public void OnCancel(BaseEventData eventData)
    {
        //tooltip.SetActive(false);
        player.HideToolTip();
        if (inventorySlot == null) return;
        if (contextMenu == null) return;

        if (contextMenu.activeSelf)
        {
            contextMenu.SetActive(false);
        }
        else
        {
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
    }
}