using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public int index;
    [SerializeField] EquipmentManager equipmentManager;
    public Image icon;
    public InventorySlotData SlotData;

    public void AddItem(InventorySlotData data)
    {
        SlotData = data;
        icon.sprite = data.item.Icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        SlotData = null;
        icon.sprite = null;
        icon.enabled = false;
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