using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public EquipmentManager equipmentManager;
    [HideInInspector] public Inventory inventory;
    public InventorySlotData slotData;

    public int slotIndex;
    public Image icon;
    public Image redTint;
    public TextMeshProUGUI amountText;

    [SerializeField] Color defaultColor;
    [SerializeField] Color upgradeColor;
    [SerializeField] Color downgradeColor;
    [SerializeField] Color SameLevelColor;

    public void AddItem(Item newItem, int quantity)
    {
        slotData = new InventorySlotData(newItem, quantity);
        inventory.items[slotIndex] = slotData;

        icon.sprite = newItem.Icon;
        icon.color = Color.white;

        // Stack Text
        amountText.text = (quantity > 1) ? quantity.ToString() : "";

        // Equipment Text
        if (newItem is Equipment equip)
        {
            amountText.text = equip.LevelRequirement.ToString();

            Equipment equippedItem = equipmentManager.currentEquipment[(int)equip.equipmentType];

            if (equippedItem == null)
            {
                amountText.color = upgradeColor;
            }
            else
            {
                if (equip.LevelRequirement > equippedItem.LevelRequirement)
                {
                    amountText.color = upgradeColor;
                }
                else if (equip.LevelRequirement < equippedItem.LevelRequirement)
                {
                    amountText.color = downgradeColor;
                }
                else
                {
                    amountText.color = SameLevelColor;
                }
            }
        }
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
        icon.color = defaultColor;

        ClearStacks();
    }

    public void RemoveItem()
    {
        if (slotData?.item != null)
        {
            inventory.RemoveItemBySlot(slotIndex);
        }
    }

    void ClearStacks()
    {
        if (amountText != null)
        {
            amountText.color = Color.white;
            amountText.text = "";
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        HandleDropFrom(fromSlot);
    }

    public void HandleDropFrom(InventorySlot fromSlot)
    {
        if (fromSlot == null || fromSlot == this)
            return;

        InventorySlot toSlot = this;

        InventorySlotData fromData = fromSlot.slotData;
        InventorySlotData toData = toSlot.slotData;

        // Swap
        fromSlot.slotData = toData;
        toSlot.slotData = fromData;

        fromSlot.inventory.items[fromSlot.slotIndex] = fromSlot.slotData;
        toSlot.inventory.items[toSlot.slotIndex] = toSlot.slotData;

        fromSlot.UpdateSlotVisuals();
        toSlot.UpdateSlotVisuals();

        fromSlot.inventory.Save.SaveInventory(
            fromSlot.slotData?.item,
            fromSlot.slotIndex,
            fromSlot.slotData?.quantity ?? 0);

        toSlot.inventory.Save.SaveInventory(
            toSlot.slotData?.item,
            toSlot.slotIndex,
            toSlot.slotData?.quantity ?? 0);
    }

    public void UpdateSlotVisuals()
    {
        if (slotData != null)
        {
            icon.sprite = slotData.item.Icon;
            icon.color = Color.white;

            // Stack Text
            amountText.text = (slotData.quantity > 1) ? slotData.quantity.ToString() : "";

            // Equipment Text
            if (slotData.item is Equipment equip)
            {
                amountText.text = equip.LevelRequirement.ToString();

                Equipment equippedItem = equipmentManager.currentEquipment[(int)equip.equipmentType];

                if (equippedItem == null)
                {
                    amountText.color = upgradeColor;
                }
                else
                {
                    if (equip.LevelRequirement > equippedItem.LevelRequirement)
                    {
                        amountText.color = upgradeColor;
                    }
                    else if (equip.LevelRequirement < equippedItem.LevelRequirement)
                    {
                        amountText.color = downgradeColor;
                    }
                    else
                    {
                        amountText.color = SameLevelColor;
                    }
                }
            }
        }
        else
        {
            icon.sprite = null;
            icon.color = defaultColor;
            amountText.text = "";
            amountText.color = Color.white;
        }
    }
}