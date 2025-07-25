using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupToolTip : MonoBehaviour
{
    [SerializeField] ItemPickup itemPickup;

    [SerializeField] ItemRarityInfo riarityInfo;
    [SerializeField] GameObject gemSlotsImage;

    [SerializeField] Image itemIcon;
    [SerializeField] Image textBox;

    [SerializeField] TextMeshProUGUI itemName_Text;
    [SerializeField] TextMeshProUGUI itemInfo_Text;

    private void Start()
    {
        if (itemPickup.Item != null)
        {
            if (itemPickup.Item is Currency currency)
            {
                UpdateCurrencyInfo(currency);
            }

            if (itemPickup.Item is Collectable collectable)
            {
                UpdateCollectableInfo(collectable);
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

    public void UpdateCurrencyInfo(Currency currency)
    {
        // Hide gem slots for currency
        gemSlotsImage.SetActive(false);

        // Sprite
        itemIcon.sprite = currency.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(itemPickup.Item.name, itemPickup.Item.ItemRarity);

        // Description
        itemInfo_Text.text = itemPickup.Item.Description;
    }

    public void UpdateCollectableInfo(Collectable collectable)
    {
        // Hide gem slots for currency
        gemSlotsImage.SetActive(false);

        // Sprite
        itemIcon.sprite = collectable.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(itemPickup.Item.name, itemPickup.Item.ItemRarity);

        // Description
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(itemPickup.Item.Description);
        sb.AppendLine($"{collectable.SellValue}<sprite index=0>");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateEquipmentInfo(Equipment equipment)
    {
        // Show gem slots for equipment
        gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = equipment.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(itemPickup.Item.name, itemPickup.Item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (var mod in equipment.modifiers)
        {
            sb.AppendLine($"+{mod.value} {mod.statType}");
        }

        sb.AppendLine();

        // Add additional info
        sb.AppendLine(FormatNameWithRarity(equipment.ItemRarity.ToString(), equipment.ItemRarity));
        sb.AppendLine($"{equipment.equipmentType}");
        sb.AppendLine($"Required Level: {equipment.LevelRequirement}");
        sb.AppendLine($"Required Class: {equipment.ClassRequirement}");
        sb.AppendLine($"{equipment.SellValue}<sprite index=0>");

        // Update text
        itemInfo_Text.text = sb.ToString();
    }

    public void UpdateWeaponInfo(Weapon weapon)
    {
        // Show gem slots for weapons
        gemSlotsImage.SetActive(true);

        // Sprite
        itemIcon.sprite = weapon.Icon;

        // Name
        itemName_Text.text = FormatNameWithRarity(itemPickup.Item.name, itemPickup.Item.ItemRarity);

        // Build text info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();

        // Loop through each stat modifier
        foreach (var mod in weapon.modifiers)
        {
            sb.AppendLine($"+{mod.value} {mod.statType}");
        }

        sb.AppendLine();

        // Add additional info
        sb.AppendLine(FormatNameWithRarity(weapon.ItemRarity.ToString(), weapon.ItemRarity));
        sb.AppendLine($"{weapon.weaponType}");
        sb.AppendLine($"Required Level: {weapon.LevelRequirement}");
        sb.AppendLine($"Required Class: {weapon.ClassRequirement}");
        sb.AppendLine($"{weapon.SellValue}<sprite index=0>");

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
            ItemRarity.Exotic => riarityInfo.ExoticColor,
            ItemRarity.Mythic => riarityInfo.MythicColor,
            ItemRarity.Legendary => riarityInfo.LegendaryColor,
            _ => Color.white // Default to white
        };

        // Assign Box Color
        textBox.color = color;

        var tempColor = textBox.color;
        tempColor.a = .90f;
        textBox.color = tempColor;

        // Convert the Color to a hex string
        string colorHex = ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{name}</b></color>";
    }
}
