using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("General")]
    public Color Muted;

    [Header("Stat Changes")]
    public Color StatIncrease;
    public Color StatDecrease;

    [Header("Quest Colors")]
    public Color Enemy;
    public Color Location;
    public Color Objective;
    public Color QuestItem;

    [Header("Icons")]
    [SerializeField] string statSpritePrefix;
    [SerializeField] string buffSpritePrefix;
    [SerializeField] string debuffSpritePrefix;
    [SerializeField] string ccSpritePrefix;
    [SerializeField] string coinSpriteName;

    [Header("Quest Keywords")]
    [SerializeField] ItemList itemList;
    [SerializeField] List<string> enemyKeywords;
    [SerializeField] List<string> locationKeywords;
    [SerializeField] List<string> objectiveKeywords;
    [SerializeField] List<string> nameKeywords;

    Dictionary<string, Func<string, string>> questStyles;
    Regex questRegex;

    Dictionary<string, Color> keywordColors;
    Regex keywordRegex;
    Regex statRegex;

    Dictionary<string, string> statusFormats;
    Regex statusRegex;

    void OnEnable() => Invalidate();
    void OnValidate() => Invalidate();
    void Invalidate() { keywordColors = null; keywordRegex = null; statRegex = null; statusFormats = null; statusRegex = null; questStyles = null; questRegex = null; }

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

    public string FormatQuest(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (questRegex == null) BuildQuestKeywords();

        return questRegex.Replace(text, m =>
            questStyles[m.Groups["word"].Value](m.Value));
    }

    void BuildQuestKeywords()
    {
        questStyles = new(StringComparer.OrdinalIgnoreCase);

        // Items first, so any list below wins if a name appears in both
        if (itemList != null)
            AddQuestWords(itemList.allItems.Where(i => i != null).Select(i => i.ITEM_NAME),
                          t => Colorize(t, QuestItem));

        AddQuestWords(enemyKeywords, t => Colorize(t, Enemy));
        AddQuestWords(locationKeywords, t => Colorize(t, Location));
        AddQuestWords(objectiveKeywords, t => Colorize(t, Objective));
        AddQuestWords(nameKeywords, t => $"<b>{t}</b>");

        if (questStyles.Count == 0) { questRegex = new Regex("(?!)"); return; } // matches nothing

        // Longest first, so "Giant Coconut" and "Coconut Sprouts" beat "Coconut"
        var keys = questStyles.Keys.OrderByDescending(k => k.Length).Select(Regex.Escape);
        questRegex = new Regex($@"\b(?<word>{string.Join("|", keys)})s?\b",
                               RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    void AddQuestWords(IEnumerable<string> words, Func<string, string> style)
    {
        if (words == null) return;
        foreach (string w in words)
            if (!string.IsNullOrWhiteSpace(w)) questStyles[w.Trim()] = style;
    }
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
