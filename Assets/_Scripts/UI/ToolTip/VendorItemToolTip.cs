using UnityEngine;
using UnityEngine.EventSystems;

public class VendorItemToolTip : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Player player;
    InventorySlotData data;

    public void Init(Player player, InventorySlotData slotData)
    {
        this.player = player;
        data = slotData;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (data == null || data.item == null)
            return;

        player.ShowToolTip(data);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (player != null) player.HideToolTip();
    }

    private void OnDestroy()
    {
        if (player != null) player.HideToolTip();
    }
}
