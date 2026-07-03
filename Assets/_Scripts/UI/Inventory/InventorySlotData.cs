
using System.Collections.Generic;

[System.Serializable]
public class InventorySlotData
{
    public Item item;
    public int quantity;
    public ItemRarity rarity;
    public ItemQuality quality;
    public List<StatModifier> modifiers;

    public InventorySlotData(Item item, int quantity, ItemRarity rarity, ItemQuality quality, List<StatModifier> modifiers = null)
    {
        this.item = item;
        this.quantity = quantity;
        this.rarity = rarity;
        this.quality = quality;
        this.modifiers = modifiers;
    }
}