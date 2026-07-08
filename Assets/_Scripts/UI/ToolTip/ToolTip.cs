using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [Header("Data")]
    InventorySlotData data;

    [Header("UI")]
    [SerializeField] GameObject tooltip;
    [SerializeField] Image itemIcon;
    [SerializeField] Image itemBackground;
    [SerializeField] Image textBox;
    [SerializeField] TextMeshProUGUI itemName_Text;
    [SerializeField] TextMeshProUGUI itemInfo_Text;

    public void GetData(InventorySlotData slotData)
    {
        data = slotData;
    }

    public void UpdateToolTip()
    {
        if (data == null) return;

        // Sprite
        itemIcon.sprite = data.item.Icon;

        // Set Background Color
        itemBackground.color = data.item.GetRarityColor(data.rarity);

        // Name
        itemName_Text.text = FormatNameWithRarity(data.item.name, data.rarity);

        // Description
        itemInfo_Text.text = FormatDescription(data);
    }

    string FormatNameWithRarity(string name, ItemRarity rarity)
    {
        // Get Rarity Color
        Color color = data.item.GetRarityColor(rarity);

        // Assign Box Color
        textBox.color = color;

        Color tempColor = textBox.color;
        tempColor.a = .80f;
        textBox.color = tempColor;

        // Convert the Color to a hex string
        string colorHex = ColorUtility.ToHtmlStringRGB(color);

        // Format the name with the appropriate color using rich text
        return $"<color=#{colorHex}><b>{name}</b></color>";
    }

    string FormatNameWithQuality(ItemQuality quality)
    {
        Color color = quality switch
        {
            ItemQuality.Normal => Color.white,
            ItemQuality.Good => new Color32(120, 200, 120, 255),
            ItemQuality.Great => new Color32(100, 170, 255, 255),
            ItemQuality.Excellent => new Color32(255, 215, 100, 255),
            _ => Color.white
        };

        string hex = ColorUtility.ToHtmlStringRGB(color);

        return $"<color=#{hex}><b>{quality}</b></color>";
    }

    string FormatDescription(InventorySlotData data)
    {
        StringBuilder sb = new();

        switch (data.item)
        {
            case Currency currency:
                sb.AppendLine(currency.Description);
                break;

            case Collectable collectable:
                sb.AppendLine(collectable.Description);
                sb.AppendLine($"{collectable.SellValue}<sprite index=0>");
                break;

            case Equipment equipment:
                foreach (StatModifier mod in data.modifiers)
                {
                    sb.AppendLine($"+{mod.value} {mod.statType}");
                }

                sb.AppendLine();

                sb.AppendLine(FormatNameWithRarity(data.rarity.ToString(), data.rarity));
                sb.AppendLine(FormatNameWithQuality(data.quality));
                if (equipment is Weapon weapon)
                {
                    sb.AppendLine(weapon.weaponType.ToString());
                }
                else
                {
                    sb.AppendLine(equipment.equipmentType.ToString());
                }
                    
                sb.AppendLine($"Required Level: {equipment.LevelRequirement}");
                sb.AppendLine($"Required Class: {equipment.ClassRequirement}");
                sb.AppendLine($"{equipment.SellValue}<sprite index=0>");
                break;
        }

        return sb.ToString();
    }
}
