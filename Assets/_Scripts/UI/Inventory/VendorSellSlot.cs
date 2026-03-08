using UnityEngine;
using UnityEngine.EventSystems;

public class VendorSellSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] Inventory inventory;

    public void OnDrop(PointerEventData eventData)
    {
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        if (fromSlot?.slotData == null) return;

        InventorySlotData data = fromSlot.slotData;

        inventory.CoinCollected(data.item.SellValue * data.quantity);
        inventory.RemoveItemBySlot(fromSlot.slotIndex, data.quantity);
    }
}
