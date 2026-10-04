using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] ConsumableSlots consumableSlots;
    public ConsumableSlotType slotType;

    [Header("UI")]
    public Image itemIcon;
    public Image itemBackground;
    public Image image_QualityBorder;
    public Image cooldownOverlay;
    public Image img_Tint;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI cooldownText;

    public void Refresh(InventorySlotData data)
    {
        // Clear the slot if there's no item
        if (data == null || data.item == null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            itemBackground.enabled = false;
            image_QualityBorder.enabled = false;
            amountText.text = "";
            return;
        }

        // Update the slot with the new item data
        itemIcon.sprite = data.item.Icon;
        itemIcon.enabled = true;

        itemBackground.color = data.item.GetRarityColor(data.rarity);
        itemBackground.enabled = true;

        image_QualityBorder.color = data.item.GetQualityColor(data.quality);
        image_QualityBorder.enabled = true;

        amountText.text = data.quantity > 1 ? data.quantity.ToString() : "";
    }

    public void UseItem()
    {
        consumableSlots.UnEquip(slotType);
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemDrag draggedItem = eventData.pointerDrag?.GetComponent<ItemDrag>();
        if (draggedItem == null) return;
        if (!draggedItem.canDrag) return;

        InventorySlot fromSlot = draggedItem.inventorySlot;
        if (fromSlot?.slotData?.item == null) return;

        // Only accept a consumable that belongs in this slot type
        if (fromSlot.slotData.item is Consumable consumable && consumable.SlotType == slotType)
        {
            consumableSlots.Equip(fromSlot.slotData);
        }
    }

    void Update()
    {
        float remaining = consumableSlots.CooldownRemaining(slotType);

        if (remaining > 0f)
        {
            cooldownOverlay.enabled = true;
            img_Tint.enabled = true;
            cooldownOverlay.fillAmount = remaining / consumableSlots.GetCooldown(slotType);

            int seconds = Mathf.CeilToInt(remaining);
            cooldownText.text = $"{seconds / 60}:{seconds % 60:00}";
        }
        else if (cooldownOverlay.enabled)
        {
            cooldownOverlay.enabled = false;
            img_Tint.enabled = false;
            cooldownText.text = "";
        }
    }
}
