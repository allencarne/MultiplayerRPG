using UnityEngine;

[CreateAssetMenu(fileName = "TooltipPalette", menuName = "Scriptable Objects/TooltipPalette")]
public class TooltipPalette : ScriptableObject
{
    [Header("Damage")]
    public Color PhysicalDamage = new Color(1f, 1f, 1f);           // white — matches "PHYSICAL DAMAGE" convention
    public Color TrueDamage = new Color(1f, 0.6f, 0.27f);          // orange — reads as "unblockable"
    public Color PercentHealthDamage = new Color(0.7f, 0.4f, 1f);  // purple — distinct "special" damage flavor
    public Color DamageRatioText = new Color(1f, 0.65f, 0f);       // orange — matches AD-style ratio text

    [Header("Healing")]
    public Color Healing = new Color(0.4f, 1f, 0.4f);              // green

    [Header("Buffs / Debuffs")]
    public Color Buff = new Color(0.3f, 0.85f, 1f);                // cyan/blue — positive status
    public Color Debuff = new Color(0.85f, 0.25f, 0.25f);          // red — negative status

    [Header("Crowd Control")]
    public Color CrowdControl = new Color(1f, 0.85f, 0.2f);        // yellow — universally reads as "danger, can't act"

    [Header("Mobility")]
    public Color Mobility = new Color(0.4f, 1f, 0.85f);            // teal — distinct from CC and buffs

    [Header("Utility")]
    public Color Utility = new Color(0.8f, 0.8f, 0.8f);            // light grey — neutral/support effects

    [Header("Resource")]
    public Color ManaCost = new Color(0.4f, 0.6f, 1f);             // blue — matches typical "mana" association
    public Color Cooldown = new Color(0.75f, 0.75f, 0.75f);        // grey — neutral, informational

    [Header("Stat Changes")]
    public Color StatIncrease = new Color(0.4f, 1f, 0.4f);         // green — matches healing (both "good")
    public Color StatDecrease = new Color(0.85f, 0.25f, 0.25f);    // red — matches debuff (both "bad")

    // Convenience: hex without the '#', ready to drop into rich text tags.
    public string Hex(Color c) => ColorUtility.ToHtmlStringRGB(c);
}
