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
        redTint.enabled = IsUnderLevelRequirement(newItem);

        RefreshAmountText(newItem, quantity);
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
        redTint.enabled = false;

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
        if (!draggedItem.canDrag) return;

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

        // Stack if same stackable item
        if (toData != null && fromData != null &&
            fromData.item.IsStackable &&
            fromData.item.name == toData.item.name)
        {
            toData.quantity += fromData.quantity;
            fromSlot.slotData = null;
            fromSlot.inventory.items[fromSlot.slotIndex] = null;

            fromSlot.inventory.Save.SaveInventory(null, fromSlot.slotIndex, 0);
            toSlot.inventory.Save.SaveInventory(toData.item, toSlot.slotIndex, toData.quantity);

            fromSlot.UpdateSlotVisuals();
            toSlot.UpdateSlotVisuals();
            return;
        }

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
            redTint.enabled = IsUnderLevelRequirement(slotData.item);

            RefreshAmountText(slotData.item, slotData.quantity);
        }
        else
        {
            icon.sprite = null;
            icon.color = defaultColor;
            amountText.text = "";
            amountText.color = Color.white;
            redTint.enabled = false;
        }
    }

    bool IsUnderLevelRequirement(Item item)
    {
        return item is Equipment equip && inventory.Stats.PlayerLevel.Value < equip.LevelRequirement;
    }

    void RefreshAmountText(Item item, int quantity)
    {
        if (item is Equipment equip)
        {
            amountText.text = equip.LevelRequirement.ToString();
            amountText.color = GetEquipmentComparisonColor(equip);
        }
        else
        {
            amountText.text = (quantity > 1) ? quantity.ToString() : "";
            amountText.color = Color.white;
        }
    }

    Color GetEquipmentComparisonColor(Equipment equip)
    {
        Equipment equipped = equipmentManager.currentEquipment[(int)equip.equipmentType];

        if (equipped == null) return upgradeColor;
        if (equip.LevelRequirement > equipped.LevelRequirement) return upgradeColor;
        if (equip.LevelRequirement < equipped.LevelRequirement) return downgradeColor;
        return SameLevelColor;
    }
}