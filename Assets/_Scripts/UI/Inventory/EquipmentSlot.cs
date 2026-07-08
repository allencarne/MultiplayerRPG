using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    // References
    [SerializeField] EquipmentManager equipmentManager;
    public InventorySlotData SlotData;

    // UI
    public Image itemIcon;
    public Image itemBackground;

    // Value
    public int index;

    public void AddItem(InventorySlotData data)
    {
        SlotData = data;

        itemIcon.sprite = data.item.Icon;
        itemIcon.enabled = true;

        itemBackground.color = data.item.GetRarityColor(data.rarity);
        itemBackground.enabled = true;
    }

    public void ClearSlot()
    {
        SlotData = null;

        itemIcon.sprite = null;
        itemIcon.enabled = false;

        itemBackground.color = Color.white;
        itemBackground.enabled = false;
    }

    public void UseItem()
    {
        if (SlotData != null) equipmentManager.UnEquip(index);
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;
        if (!draggedItem.canDrag) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        if (fromSlot?.slotData?.item == null) return;

        // Only accept equipment that belongs to this slot type
        if (fromSlot.slotData.item is Equipment equip && (int)equip.equipmentType == index)
        {
            equipmentManager.Equip(fromSlot.slotData);
        }
    }
}