using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/RichTextTheme")]
public class RichTextTheme : ScriptableObject
{
    [Header("Stats")]
    public Color Health;
    public Color HealthRegen;
    public Color Recharge;
    public Color Dexterity;
    public Color Endurance;
    public Color EnduranceRegen;
    public Color Precision;
    public Color Ferocity;
    public Color Power;
    public Color Vamp;
    public Color Armor;
    public Color Lethality;
    public Color Mana;
    public Color ManaRegen;
    public Color Speed;

    [Header("Damage")]
    public Color PhysicalDamage;
    public Color TrueDamage;
    public Color PercentHealthDamage;
    public Color DamageRatioText;

    [Header("Healing")]
    public Color Healing;

    [Header("Buffs / Debuffs")]
    public Color Buff;
    public Color Debuff;

    [Header("Crowd Control")]
    public Color CrowdControl;

    [Header("Mobility")]
    public Color Mobility;

    [Header("Utility")]
    public Color Utility;

    [Header("Resource")]
    public Color ManaCost;
    public Color Cooldown;

    [Header("Stat Changes")]
    public Color StatIncrease;
    public Color StatDecrease;

    [Header("Icons")]
    [SerializeField] string statSpritePrefix;
    [SerializeField] string coinSpriteName;

    Dictionary<string, Color> keywordColors;
    Regex keywordRegex;
    Regex statRegex;

    void OnEnable() => Invalidate();
    void OnValidate() => Invalidate();
    void Invalidate() { keywordColors = null; keywordRegex = null; statRegex = null; }

    public Color GetStatColor(StatType stat) => stat switch
    {
        StatType.Health => Health,
        StatType.HealthRegen => HealthRegen,
        StatType.Recharge => Recharge,
        StatType.Dexterity => Dexterity,
        StatType.Endurance => Endurance,
        StatType.EnduranceRegen => EnduranceRegen,
        StatType.Precision => Precision,
        StatType.Ferocity => Ferocity,
        StatType.Power => Power,
        StatType.Vamp => Vamp,
        StatType.Armor => Armor,
        StatType.Lethality => Lethality,
        StatType.Mana => Mana,
        StatType.ManaRegen => ManaRegen,
        StatType.Speed => Speed,
        _ => Color.white
    };

    public string Format(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return ApplyStatTokens(ApplyKeywords(text));
    }

    public string Hex(Color c) => ColorUtility.ToHtmlStringRGB(c);

    public string Colorize(string text, Color color) => $"<color=#{Hex(color)}>{text}</color>";

    public string StatIcon(StatType stat) => $"<sprite name=\"{statSpritePrefix}{stat}\">";

    public string CoinIcon => $"<sprite name=\"{coinSpriteName}\">";

    public string Stat(StatType stat) => $"{StatIcon(stat)} {Colorize(stat.ToString(), GetStatColor(stat))}";

    public string FormatModifier(StatModifier mod)
    {
        bool percent = mod.modType == ModType.Percent;
        float v = percent ? mod.value * 100f : mod.value;
        string sign = v >= 0 ? "+" : "";
        string suffix = percent ? "%" : "";
        return $"{sign}{v:0.##}{suffix} {Stat(mod.statType)}";
    }

    string ApplyKeywords(string text)
    {
        if (keywordRegex == null) BuildKeywords();
        return keywordRegex.Replace(text, m => Colorize(m.Value, keywordColors[m.Value]));
    }

    public string ApplyStatTokens(string text)
    {
        statRegex ??= new Regex($@"\b({string.Join("|", Enum.GetNames(typeof(StatType)))})\b",RegexOptions.Compiled);
        return statRegex.Replace(text, m => Stat(Enum.Parse<StatType>(m.Value)));
    }

    public string ApplyPaletteTokens(string text)
    {
        if (keywordRegex == null) BuildKeywords();
        return keywordRegex.Replace(text, m => $"<color=#{Hex(keywordColors[m.Value])}>{m.Value}</color>");
    }

    void BuildKeywords()
    {
        keywordColors = new()
        {
            ["Damage"] = PhysicalDamage,
            ["Damaging"] = PhysicalDamage,
            ["Heal"] = Healing,
            ["Healing"] = Healing,
            ["Buff"] = Buff,
            ["Buffing"] = Buff,
            ["Protection"] = Buff,
            ["Regeneration"] = Buff,
            ["Replenishment"] = Buff,
            ["Resurgence"] = Buff,
            ["Debuff"] = Debuff,
            ["Debuffing"] = Debuff,
            ["Bleed"] = Debuff,
            ["Bleeding"] = Debuff,
            ["Stun"] = CrowdControl,
            ["Stunning"] = CrowdControl,
            ["Slow"] = CrowdControl,
            ["Slowing"] = CrowdControl,
            ["Dash"] = Mobility,
            ["Leap"] = Mobility,
            ["Jump"] = Mobility,
            ["Flailing Edge"] = Utility,
            ["Heavy Strike"] = Utility,
            ["Mending Field"] = Utility
        };

        keywordRegex = new Regex($@"\b({string.Join("|", keywordColors.Keys)})\b", RegexOptions.Compiled);
    }
}
