
[System.Serializable]
public class InventorySlotData
{
    public Item item;
    public int quantity;
    public ItemRarity rarity;
    public ItemQuality quality;

    public InventorySlotData(Item item, int quantity, ItemRarity rarity, ItemQuality quality)
    {
        this.item = item;
        this.quantity = quantity;
        this.rarity = rarity;
        this.quality = quality;
    }
}