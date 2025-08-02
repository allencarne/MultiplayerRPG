
[System.Serializable]
public class InventorySlotData
{
    public Item item;
    public int quantity;

    public InventorySlotData(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}