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

    [Header("Status Effects")]
    public Color Buff;
    public Color Debuff;
    public Color CrowdControl;

    [Header("Mobility")]
    public Color Mobility;

    [Header("Utility")]
    public Color Utility;

    [Header("Stat Changes")]
    public Color StatIncrease;
    public Color StatDecrease;

    [Header("Icons")]
    [SerializeField] string statSpritePrefix;
    [SerializeField] string buffSpritePrefix;
    [SerializeField] string debuffSpritePrefix;
    [SerializeField] string ccSpritePrefix;
    [SerializeField] string coinSpriteName;

    Dictionary<string, Color> keywordColors;
    Regex keywordRegex;
    Regex statRegex;

    Dictionary<string, string> statusFormats;
    Regex statusRegex;

    void OnEnable() => Invalidate();
    void OnValidate() => Invalidate();
    void Invalidate() { keywordColors = null; keywordRegex = null; statRegex = null; statusFormats = null; statusRegex = null; }

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
        return ApplyStatusTokens(ApplyStatTokens(ApplyKeywords(text)));
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
            ["Heal"] = Health,
            ["Healing"] = Health,
            ["Buff"] = Buff,
            ["Buffing"] = Buff,
            ["Debuff"] = Debuff,
            ["Debuffing"] = Debuff,
            ["Dash"] = Mobility,
            ["Leap"] = Mobility,
            ["Jump"] = Mobility,
            ["Flailing Edge"] = Utility,
            ["Heavy Strike"] = Utility,
            ["Mending Field"] = Utility
        };

        keywordRegex = new Regex($@"\b({string.Join("|", keywordColors.Keys)})\b", RegexOptions.Compiled);
    }

    public string ApplyStatusTokens(string text)
    {
        if (statusRegex == null) BuildStatus();
        return statusRegex.Replace(text, m => statusFormats[m.Value]);
    }

    void BuildStatus()
    {
        statusFormats = new();

        foreach (BuffType b in Enum.GetValues(typeof(BuffType)))
            statusFormats[b.ToString()] = StatusText(buffSpritePrefix, b.ToString(), Buff);

        foreach (DebuffType d in Enum.GetValues(typeof(DebuffType)))
            statusFormats[d.ToString()] = StatusText(debuffSpritePrefix, d.ToString(), Debuff);

        foreach (ControlType c in Enum.GetValues(typeof(ControlType)))
            statusFormats[c.ToString()] = StatusText(ccSpritePrefix, c.ToString(), CrowdControl);

        statusRegex = new Regex($@"\b({string.Join("|", statusFormats.Keys)})\b", RegexOptions.Compiled);
    }

    string StatusText(string prefix, string name, Color color) => $"<sprite name=\"{prefix}{name}\"> {Colorize(name, color)}";
}

public enum BuffType
{
    Might,
    Haste,
    Alacrity,
    Protection,
    Agility,
    Phase,
    Immovable,
    Immunity,
    Regeneration,
    Replenishment,
    Resurgence
}

public enum DebuffType
{
    Weakness,
    Slow,
    Impede,
    Vulnerability,
    Exhaust,
    Bleed
}

public enum ControlType
{
    Immobilize,
    Silence,
    Disarm,
    KnockBack,
    KnockUp,
    Stun,
    Pull,
    Incapacitate
}
