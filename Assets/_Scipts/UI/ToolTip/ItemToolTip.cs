using UnityEngine;
using TMPro;
using System.Text;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] InventoryItem inventoryItem;

    [SerializeField] ItemRarityInfo riarityInfo;
    [SerializeField] GameObject gemSlotsImage;
    [SerializeField] TextMeshProUGUI itemInfo_Text;

    private void Start()
    {
        if (inventoryItem.Item != null)
        {
            if (inventoryItem.Item is Currency)
            {
                UpdateCurrencyInfo();
            }

            if (inventoryItem.Item is Equipment equipment)
            {
                UpdateEquipmentInfo(equipment);
            }

            if (inventoryItem.Item is Weapon weapon)
            {
                UpdateWeaponInfo(weapon);
            }
        }
    }

    public void UpdateCurrencyInfo()
    {
        // Hide gem slots for currency
        gemSlotsImage.SetActive(false);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatNameWithRarity(inventoryItem.Item.Prefab.name, inventoryItem.Item.ItemRarity));
        sb.AppendLine();
        sb.AppendLine(inventoryItem.Item.Description); // Description

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateEquipmentInfo(Equipment equipment)
    {
        // Show gem slots for equipment
        gemSlotsImage.SetActive(true);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatNameWithRarity(equipment.Prefab.name, equipment.ItemRarity));
        sb.AppendLine();

        /*
        // Add modifiers (hide if 0)
        if (equipment.healthModifier != 0)
        {
            sb.AppendLine($"+{equipment.healthModifier} Health");
        }
        if (equipment.damageModifier != 0)
        {
            sb.AppendLine($"+{equipment.damageModifier} Health");
        }
        sb.AppendLine();
        */

        // Add additional info
        //sb.AppendLine($"{equipment.ItemRarity}");
        sb.AppendLine(FormatNameWithRarity(equipment.ItemRarity.ToString(),equipment.ItemRarity));
        sb.AppendLine($"{equipment.equipmentType}");
        sb.AppendLine($"Level Req: {equipment.LevelRequirement}");
        sb.AppendLine($"Class Req: {equipment.ClassRequirement}");
        sb.AppendLine($"Sell Value: {equipment.SellValue}");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateWeaponInfo(Weapon weapon)
    {
        // Show gem slots for weapons
        gemSlotsImage.SetActive(true);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(FormatNameWithRarity(weapon.Prefab.name, weapon.ItemRarity));
        sb.AppendLine();

        /*
        // Add modifiers (hide if 0)
        if (weapon.healthModifier != 0)
        {
            sb.AppendLine($"+{weapon.healthModifier} Health");
        }
        if (weapon.damageModifier != 0)
        {
            sb.AppendLine($"+{weapon.damageModifier} Health");
        }
        sb.AppendLine();
        */

        // Add additional info
        //sb.AppendLine($"{weapon.ItemRarity}");
        sb.AppendLine(FormatNameWithRarity(weapon.ItemRarity.ToString(), weapon.ItemRarity));
        sb.AppendLine($"{weapon.weaponType}");
        sb.AppendLine($"Level Req: {weapon.LevelRequirement}");
        sb.AppendLine($"Class Req: {weapon.ClassRequirement}");
        sb.AppendLine($"Sell Value: {weapon.SellValue}");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    private string FormatNameWithRarity(string name, ItemRarity rarity)
    {
        // Retrieve the appropriate color from the ItemRarityInfo
        Color color = rarity switch
        {
            ItemRarity.Common => riarityInfo.CommonColor,
            ItemRarity.Uncommon => riarityInfo.UnCommonColor,
            ItemRarity.Rare => riarityInfo.RareColor,
            ItemRarity.Epic => riarityInfo.EpicColor,
            ItemRarity.Legendary => riarityInfo.LegendaryColor,
            _ => Color.white // Default to white
        };

        // Convert the Color to a hex string
        string colorHex = ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{name}</b></color>";
    }
}
