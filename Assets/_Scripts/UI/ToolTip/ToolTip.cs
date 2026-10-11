using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] RichTextTheme palette;

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
            itemName_Text.text = FormatNameWithRarity(data.item.ITEM_NAME, data.rarity);

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
                sb.AppendLine(palette.Format(skillData.Description.Trim()));
                sb.AppendLine();
            }

            DamageEffect dmg = skillData.FindDamageEffect();
            if (dmg != null)
            {
                sb.AppendLine();
                sb.AppendLine(FormatDamageLine(dmg));
            }

            sb.AppendLine($"<color=#{palette.Hex(palette.Recharge)}>{palette.StatIcon(StatType.Recharge)} Cooldown: {skillData.CoolDown:0.##}s</color>");
            sb.AppendLine($"<color=#{palette.Hex(palette.Mana)}>{palette.StatIcon(StatType.Mana)} Mana Cost: {skillData.ManaCost}</color>");

            float? castTime = skillData.GetCastTime();
            if (castTime.HasValue)
            {
                sb.AppendLine($"Cast Time: {castTime.Value:0.##}s");
            }

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
        string icon = palette.StatIcon(mod.statType);

        if (mod.modType == ModType.Percent)
        {
            float pct = mod.value * 100f;
            string sign = pct >= 0 ? "+" : "";
            return $"{sign}{pct:0.##}% {icon} {mod.statType}";
        }
        else
        {
            string sign = mod.value >= 0 ? "+" : "";
            return $"{sign}{mod.value:0.##} {icon} {mod.statType}";
        }
    }

    void OnPlayerLevelChanged(int oldVal, int newVal)
    {
        UpdateToolTip();
    }

    string FormatDamageLine(DamageEffect dmg)
    {
        bool isPercent = dmg.DamageType != DamageType.Flat && dmg.DamageType != DamageType.True;
        string suffix = isPercent ? "%" : "";

        bool isTrue = dmg.DamageType is DamageType.True
                      or DamageType.PercentMaxHealthTrue
                      or DamageType.PercentMissingHealthTrue
                      or DamageType.PercentCurrentHealthTrue;

        string label = dmg.DamageType switch
        {
            DamageType.PercentMaxHealth or DamageType.PercentMaxHealthTrue => "Max Health Damage",
            DamageType.PercentMissingHealth or DamageType.PercentMissingHealthTrue => "Missing Health Damage",
            DamageType.PercentCurrentHealth or DamageType.PercentCurrentHealthTrue => "Current Health Damage",
            _ => "Damage"
        };
        if (isTrue) label = "True " + label;

        Color labelColor = dmg.DamageType switch
        {
            DamageType.Flat => palette.PhysicalDamage,
            DamageType.True => palette.TrueDamage,
            _ => palette.PercentHealthDamage
        };

        // Breakdown: "50 + 100% [icon] Power + 2 per level"
        List<string> parts = new();
        if (dmg.BaseDamage != 0) parts.Add($"{dmg.BaseDamage:0.##}{suffix}");
        if (dmg.DamageRatio != 0) parts.Add($"{dmg.DamageRatio * 100f:0.##}% {palette.Stat(StatType.Power)}");
        if (dmg.PerLevelBonus != 0) parts.Add($"{dmg.PerLevelBonus:0.##} per level");

        StringBuilder sb = new();

        if (stats != null)
        {
            float computed = dmg.BaseDamage + (stats.TotalDamage * dmg.DamageRatio) + (stats.PlayerLevel.Value * dmg.PerLevelBonus);
            sb.Append($"<b>{computed:0.#}{suffix}</b> ");
        }

        sb.Append(palette.Colorize(label, labelColor));

        if (parts.Count > 0)
        {
            string breakdown = $"({string.Join(" + ", parts)})";
            sb.Append($" <size=90%>{palette.Colorize(breakdown, palette.Muted)}</size>");
        }

        return sb.ToString();
    }
}
