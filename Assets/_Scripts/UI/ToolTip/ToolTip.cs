using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [Header("Data")]
    InventorySlotData data;

    [Header("UI")]
    [SerializeField] GameObject tooltip;
    [SerializeField] Image itemIcon;
    [SerializeField] Image itemBackground;
    [SerializeField] Image textBox;
    [SerializeField] Image image_QualityBorder;
    [SerializeField] TextMeshProUGUI itemName_Text;
    [SerializeField] TextMeshProUGUI itemInfo_Text;

    private void OnEnable()
    {
        if (stats != null)
        {
            stats.PlayerLevel.OnValueChanged += OnPlayerLevelChanged;
        }
    }

    private void OnDisable()
    {
        if (stats != null)
        {
            stats.PlayerLevel.OnValueChanged -= OnPlayerLevelChanged;
        }
    }

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

        // Check Level Requirement
        itemIcon.color = IsUnderLevelRequirement(data.item) ? Color.red : Color.white;
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
        // Get Quality Color
        Color color = data.item.GetQualityColor(quality);

        // Assign Border Color
        image_QualityBorder.color = color;

        // Convert the Color to a hex string
        string hex = ColorUtility.ToHtmlStringRGB(color);

        // Format the quality with the appropriate color using rich text
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

                bool underLevel = stats.PlayerLevel.Value < equipment.LevelRequirement;
                string levelColor = underLevel ? "red" : "white";

                bool wrongClass = IsWrongClass(equipment);
                string classColor = wrongClass ? "red" : "white";


                sb.AppendLine($"<color={levelColor}>Required Level: {equipment.LevelRequirement}</color>");
                sb.AppendLine($"<color={classColor}>Required Class: {equipment.ClassRequirement}</color>");
                sb.AppendLine($"{equipment.SellValue}<sprite index=0>");
                break;
        }

        return sb.ToString();
    }

    void OnPlayerLevelChanged(int oldVal, int newVal)
    {
        UpdateToolTip();
    }

    bool IsUnderLevelRequirement(Item item)
    {
        return item is Equipment equip && stats.PlayerLevel.Value < equip.LevelRequirement;
    }

    bool IsWrongClass(Item item)
    {
        // Check if the item is an Equipment
        if (item is not Equipment equipment) return false;

        // Check if the equipment has a class requirement
        if (equipment.ClassRequirement == ClassRequirement.None) return false;

        // Check if the player's class matches the equipment's class requirement
        return stats.playerClass.ToString() != equipment.ClassRequirement.ToString();
    }
}
