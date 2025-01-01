using UnityEngine;
using TMPro;
using System.Text;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] ItemPickup itemPickup;

    [SerializeField] GameObject gemSlotsImage;
    [SerializeField] TextMeshProUGUI itemInfo_Text;


    private void Start()
    {
        if (itemPickup.Item != null)
        {
            if (itemPickup.Item.IsCurrency)
            {
                UpdateCurrencyInfo();
            }

            if (itemPickup.Item is Equipment equipment)
            {
                UpdateEquipmentInfo(equipment);
            }

            if (itemPickup.Item is Weapon weapon)
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
        sb.AppendLine(FormatNameWithRarity(itemPickup.Item.Prefab.name, itemPickup.Item.ItemRarity));
        sb.AppendLine();
        sb.AppendLine(itemPickup.Item.Description); // Description

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

        // Add additional info
        sb.AppendLine($"{equipment.ItemRarity}");
        sb.AppendLine($"{equipment.equipmentType}");
        sb.AppendLine($"Level Req: {equipment.LevelRequirement}");
        sb.AppendLine($"Class Req: {equipment.ClassRequirement}");
        sb.AppendLine($"Sell Value: {equipment.SellValue} Gold");

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

        // Add additional info
        sb.AppendLine($"{weapon.ItemRarity}");
        sb.AppendLine($"{weapon.weaponType}");
        sb.AppendLine($"Level Req: {weapon.LevelRequirement}");
        sb.AppendLine($"Class Req: {weapon.ClassRequirement}");
        sb.AppendLine($"Sell Value: {weapon.SellValue} Gold");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    private string FormatNameWithRarity(string name, ItemRarity rarity)
    {
        // Determine the color based on rarity
        string color = rarity switch
        {
            ItemRarity.Common => "#808080",      // Gray
            ItemRarity.Uncommon => "#00FF00",   // Green
            ItemRarity.Rare => "#0000FF",       // Blue
            ItemRarity.Epic => "#800080",       // Purple
            ItemRarity.Legendary => "#FFFF00",  // Yellow
            _ => "#FFFFFF"                      // Default White
        };

        // Format the name with the appropriate color using rich text
        return $"<color={color}><b>{name}</b></color>";
    }
}
