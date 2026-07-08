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
        slotData = data;

        // Set Item Icon
        itemIcon.sprite = data.item.Icon;
        itemIcon.enabled = true;

        // Set Item Background color based on rarity
        itemBackground.color = data.item.GetRarityColor(data.rarity);

        // Set Red Tint if we are Under Level
        redTint.enabled = IsUnderLevelRequirement(data.item);

        // Displays Item Quantity
        RefreshAmountText(data.item, data.quantity);
    }

    public void UseItem()
    {
        if (slotData?.item != null)
        {
            slotData.item.Use(inventory, equipmentManager, slotData);
        }
    }

    public void ClearSlot()
    {
        slotData = null;
        inventory.items[slotIndex] = null;

        itemIcon.sprite = null;
        itemIcon.enabled = false;

        itemBackground.color = defaultColor;

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
        if (toData != null && fromData != null && fromData.item.IsStackable && fromData.item.name == toData.item.name && fromData.rarity == toData.rarity && fromData.quality == toData.quality)
        {
            toData.quantity += fromData.quantity;
            fromSlot.slotData = null;
            fromSlot.inventory.items[fromSlot.slotIndex] = null;

            fromSlot.inventory.Save.SaveInventory(null, fromSlot.slotIndex);
            toSlot.inventory.Save.SaveInventory(toData, toSlot.slotIndex);

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

        fromSlot.inventory.Save.SaveInventory(fromSlot.slotData, fromSlot.slotIndex);
        toSlot.inventory.Save.SaveInventory(toSlot.slotData, toSlot.slotIndex);
    }

    public void UpdateSlotVisuals()
    {
        if (slotData != null)
        {
            // Set Item Icon
            itemIcon.sprite = slotData.item.Icon;
            itemIcon.enabled = true;

            // Set Item Background color based on rarity
            itemBackground.color = slotData.item.GetRarityColor(slotData.rarity);

            // Set Red Tint if we are under level
            redTint.enabled = IsUnderLevelRequirement(slotData.item);

            // Display Item Quantity
            RefreshAmountText(slotData.item, slotData.quantity);
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;

            itemBackground.color = defaultColor;
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
        Equipment equipped = equipmentManager.currentEquipment[(int)equip.equipmentType]?.item as Equipment;

        if (equipped == null) return upgradeColor;
        if (equip.LevelRequirement > equipped.LevelRequirement) return upgradeColor;
        if (equip.LevelRequirement < equipped.LevelRequirement) return downgradeColor;
        return SameLevelColor;
    }
}