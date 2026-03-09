using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public int index;
    [SerializeField] EquipmentManager equipmentManager;
    public Image icon;
    public Item Item;

    public void AddItem(Item newItem)
    {
        Item = newItem;

        icon.sprite = Item.Icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        Item = null;

        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (Item != null)
        {
            equipmentManager.UnEquip(index);
        }
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
            equipmentManager.Equip(equip);
        }
    }
}