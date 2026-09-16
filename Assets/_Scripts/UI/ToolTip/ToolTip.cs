using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [Header("Data")]
    InventorySlotData data;
    SkillData skillData;

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
        skillData = null;
    }

    public void GetData(SkillData skill)
    {
        skillData = skill;
        data = null;
    }

    public void UpdateToolTip()
    {
        if (data == null && skillData == null) return;

        if (data != null)
        {
            // --- Item rendering (existing behavior) ---
            // Sprite
            itemIcon.sprite = data.item.Icon;

            // Set Background Color
            itemBackground.color = data.item.GetRarityColor(data.rarity);

            // Name
            itemName_Text.text = FormatNameWithRarity(data.item.name, data.rarity);

            // Description
            itemInfo_Text.text = FormatDescription(data);

            // Show a red tint if the player is too low level to use this item
            if (data.item is Equipment equipment)
            {
                itemIcon.color = equipment.CanPlayerUse(stats) ? Color.white : Color.red;
            }
            else
            {
                itemIcon.color = Color.white;
            }
        }
        else if (skillData != null)
        {
            // --- Skill rendering (new) ---
            itemIcon.sprite = skillData.Icon;

            // Skill tooltip doesn't have item rarity metadata — use neutral colors
            Color box = Color.white;
            box.a = 0.8f;
            textBox.color = box;

            itemBackground.color = Color.white;
            image_QualityBorder.color = Color.clear;

            // Name and description
            // Use Name property if present otherwise fallback to ScriptableObject name
            string displayName = string.IsNullOrEmpty(skillData.Name) ? skillData.name : skillData.Name;
            itemName_Text.text = $"<b>{displayName}</b>";

            StringBuilder sb = new();
            if (!string.IsNullOrEmpty(skillData.Description))
            {
                sb.AppendLine(skillData.Description.Trim());
                sb.AppendLine();
            }

            sb.AppendLine($"Cooldown: {skillData.CoolDown:0.##}s");

            itemInfo_Text.text = sb.ToString();

            // Icon color default
            itemIcon.color = Color.white;
        }
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
                    sb.AppendLine(FormatModifierLine(mod));
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

                bool underLevel = equipment.IsUnderLevelRequirement(stats);
                string levelColor = underLevel ? "red" : "white";

                bool wrongClass = equipment.IsWrongClass(stats);
                string classColor = wrongClass ? "red" : "white";


                sb.AppendLine($"<color={levelColor}>Required Level: {equipment.LevelRequirement}</color>");
                sb.AppendLine($"<color={classColor}>Required Class: {equipment.ClassRequirement}</color>");
                sb.AppendLine($"{equipment.SellValue}<sprite index=0>");
                break;
        }

        return sb.ToString();
    }

    string FormatModifierLine(StatModifier mod)
    {
        if (mod.modType == ModType.Percent)
        {
            float pct = mod.value * 100f;
            string sign = pct >= 0 ? "+" : "";
            return $"{sign}{pct:0.##}% {mod.statType}";
        }
        else
        {
            string sign = mod.value >= 0 ? "+" : "";
            return $"{sign}{mod.value:0.##} {mod.statType}";
        }
    }

    void OnPlayerLevelChanged(int oldVal, int newVal)
    {
        UpdateToolTip();
    }
}
