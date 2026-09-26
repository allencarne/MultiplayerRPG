using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] TooltipPalette palette;

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
            // --- Item rendering ---
            // Sprite
            itemIcon.sprite = data.item.Icon;

            // Set Background Color
            itemBackground.enabled = true;
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
            // --- Skill rendering ---
            itemIcon.sprite = skillData.Icon;

            // Use neutral colors
            Color box = Color.white;
            box.a = 0.8f;
            textBox.color = box;

            itemBackground.enabled = false;
            image_QualityBorder.color = Color.clear;

            // Name and description
            // Use Name property if present otherwise fallback to ScriptableObject name
            string displayName = string.IsNullOrEmpty(skillData.Name) ? skillData.name : skillData.Name;
            itemName_Text.text = $"<b>{displayName}</b>";

            StringBuilder sb = new();
            if (!string.IsNullOrEmpty(skillData.Description))
            {
                sb.AppendLine(ApplyPaletteTokens(skillData.Description.Trim()));
                sb.AppendLine();
            }

            DamageEffect dmg = skillData.FindDamageEffect();
            if (dmg != null)
            {
                sb.AppendLine();
                sb.AppendLine(FormatDamageLine(dmg));
            }

            sb.AppendLine($"<color=#{palette.Hex(palette.Cooldown)}>Cooldown: {skillData.CoolDown:0.##}s</color>");
            sb.AppendLine($"<color=#{palette.Hex(palette.ManaCost)}>Mana Cost: {skillData.ManaCost}</color>");

            itemInfo_Text.text = sb.ToString();

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

    string FormatDamageLine(DamageEffect dmg)
    {
        string header = dmg.DamageType switch
        {
            DamageType.Flat => "PHYSICAL DAMAGE",
            DamageType.True => "TRUE DAMAGE",
            DamageType.PercentMaxHealth or DamageType.PercentMaxHealthTrue => "MAX HEALTH DAMAGE",
            DamageType.PercentMissingHealth or DamageType.PercentMissingHealthTrue => "MISSING HEALTH DAMAGE",
            DamageType.PercentCurrentHealth or DamageType.PercentCurrentHealthTrue => "CURRENT HEALTH DAMAGE",
            _ => "DAMAGE"
        };

        Color headerColor = dmg.DamageType switch
        {
            DamageType.Flat => palette.PhysicalDamage,
            DamageType.True => palette.TrueDamage,
            _ => palette.PercentHealthDamage
        };

        bool isPercentBased = dmg.DamageType != DamageType.Flat && dmg.DamageType != DamageType.True;
        string suffix = isPercentBased ? "%" : "";

        StringBuilder line = new();
        line.Append($"<color=#{palette.Hex(headerColor)}><b>{header}:</b></color>\n");

        if (dmg.BaseDamage != 0) line.Append($"{dmg.BaseDamage:0.##}{suffix}");

        if (dmg.DamageRatio != 0)
        {
            if (dmg.BaseDamage != 0) line.Append(" ");
            line.Append($"<color=#{palette.Hex(palette.DamageRatioText)}>(+ {dmg.DamageRatio * 100f:0.##}% Total Damage)</color>");
        }

        if (dmg.PerLevelBonus != 0)
            line.Append($" (+ {dmg.PerLevelBonus:0.##} per level)");

        if (stats != null)
        {
            float computed = dmg.BaseDamage + (stats.TotalDamage * dmg.DamageRatio) + (stats.PlayerLevel.Value * dmg.PerLevelBonus);
            line.Append($" = <b>{computed:0.#}{suffix}</b>");
        }

        return line.ToString();
    }

    string ApplyPaletteTokens(string text)
    {
        text = text.Replace("{buff}", $"<color=#{palette.Hex(palette.Buff)}>")
                   .Replace("{/buff}", "</color>");
        text = text.Replace("{debuff}", $"<color=#{palette.Hex(palette.Debuff)}>")
                   .Replace("{/debuff}", "</color>");
        text = text.Replace("{damage}", $"<color=#{palette.Hex(palette.PhysicalDamage)}>")
                   .Replace("{/damage}", "</color>");
        text = text.Replace("{heal}", $"<color=#{palette.Hex(palette.Healing)}>")
                   .Replace("{/heal}", "</color>");
        text = text.Replace("{cc}", $"<color=#{palette.Hex(palette.CrowdControl)}>")
                   .Replace("{/cc}", "</color>");
        text = text.Replace("{mobility}", $"<color=#{palette.Hex(palette.Mobility)}>")
                   .Replace("{/mobility}", "</color>");
        text = text.Replace("{utility}", $"<color=#{palette.Hex(palette.Utility)}>")
                   .Replace("{/utility}", "</color>");

        return text;
    }
}
