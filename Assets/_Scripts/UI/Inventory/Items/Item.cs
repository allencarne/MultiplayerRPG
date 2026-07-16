using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    // Unique identifier for the item
    public string ITEM_ID;

    // Prefab reference for the item, used for instantiation in the game world
    public GameObject Prefab;

    // Display name of the item, shown in the UI
    public Sprite Icon;

    // Description of the item, providing details about its use or lore
    public string Description;

    // The value at which the item can be sold to vendors or other players
    public int SellValue;

    // The cost to acquire the item, either through crafting, purchasing, or other means
    public int Cost;

    // The chance of the item dropping from enemies or loot sources, expressed as a percentage
    [Range(0f, 100f)] public float DropChance;

    // Indicates whether multiple instances of the item can be stacked in the inventory
    public bool IsStackable;

    // Item Category
    public ItemCategory ItemCategory;

    // rarity level of the item, affecting its desirability and potential power
    public ItemRarity ItemRarity;

    // quality level of the item, affecting its effectiveness and value
    public ItemQuality ItemQuality;

    public ItemRarityInfo rarityInfo;
    public ItemStatRules ItemStatRules;

    public virtual void Use(Inventory _inventory, EquipmentManager _equipmentManager, InventorySlotData slotData)
    {

    }

    public void RemoveFromInventory(Inventory _inventory)
    {
        _inventory.RemoveItem(this);
    }

    public Color GetRarityColor(ItemRarity rarity)
    {
        Color color = Color.white;
        switch (rarity)
        {
            case ItemRarity.Common: color = rarityInfo.CommonColor; break;
            case ItemRarity.Uncommon: color = rarityInfo.UnCommonColor; break;
            case ItemRarity.Rare: color = rarityInfo.RareColor; break;
            case ItemRarity.Epic: color = rarityInfo.EpicColor; break;
            case ItemRarity.Exotic: color = rarityInfo.ExoticColor; break;
            case ItemRarity.Mythic: color = rarityInfo.MythicColor; break;
            case ItemRarity.Ascended: color = rarityInfo.AscendedColor; break;
            case ItemRarity.Legendary: color = rarityInfo.LegendaryColor; break;
        }
        return color;
    }
}

public enum ItemCategory
{
    Weapon,
    Equipment,
    Consumable,
    Collectable,
    Currency
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Exotic,
    Mythic,
    Ascended,
    Legendary
}

public enum ItemQuality { 
    Normal,
    Good,
    Great,
    Excellent
}