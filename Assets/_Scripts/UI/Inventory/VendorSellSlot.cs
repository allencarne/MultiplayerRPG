using UnityEngine;
using UnityEngine.EventSystems;

public class VendorSellSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] Inventory inventory;
    [SerializeField] VendorInfoPanel vendorInfoPanel;

    public void OnDrop(PointerEventData eventData)
    {
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        if (fromSlot?.slotData == null) return;

        vendorInfoPanel.SellAttempt(fromSlot, fromSlot.slotData.item);
    }
}
