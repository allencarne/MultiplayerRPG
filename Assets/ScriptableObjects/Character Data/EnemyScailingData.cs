using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScailingData", menuName = "Scriptable Objects/Character/EnemyScailingData")]
public class EnemyScailingData : ScriptableObject
{
    [Header("Level 1 Baseline")]
    public float BaseHealth;
    public float BaseDamage;
    public float BaseExp;

    [Header("Giant Level 1 Baseline")]
    public float GiantHealth;
    public float GiantDamage;
    public float GiantExp;
    public float GiantArmorMult;

    [Header("Dummy")]
    public float DummyHealth;

    [Header("Growth")]
    public float HealthGrowth;
    public float DamageGrowth;
    public float ExpGrowth;
    public float ArmorPerLevel;

    [Header("Multipliers")]
    public float HealthMult;
    public float DamageMult;
    public float ArmorMult;
    public float ExpMult;

    [Header("Giant Drops")]
    [Tooltip("Added to the rarity decay factor when rolling drops. Higher = better chance at rare items.")]
    [Range(0f, 0.5f)] public float GiantRarityBoost;

    [Header("Currency / Collectable Drops")]
    [Tooltip("Max amount a regular enemy drops at level 1")]
    public int BaseCurrencyMax;
    [Tooltip("Giants drop this many times more than a regular enemy")]
    public float GiantCurrencyMult;
    [Tooltip("Extra % per level (0.15 = +15% per level, same as your chests)")]
    public float CurrencyGrowth;
    [Tooltip("The minimum drop is this % of the max, so higher levels aren't 1-to-huge")]
    [Range(0f, 1f)] public float CurrencyMinPercent;

    bool IsGiant(EnemyType type) => type == EnemyType.Giant;

    public float GetHealth(int level, EnemyType type)
    {
        if (type == EnemyType.Dummy) return DummyHealth;

        return (IsGiant(type) ? GiantHealth : BaseHealth) * Mathf.Pow(1f + HealthGrowth, level - 1) * HealthMult;
    }

    public float GetArmor(int level, EnemyType type)
    {
        if (type == EnemyType.Dummy) return 0f;

        return ArmorPerLevel * level * (IsGiant(type) ? GiantArmorMult : 1f) * ArmorMult;
    }

    public float GetDamage(int level, EnemyType type)
    {
        if (type == EnemyType.Dummy) return 0f;

        return (IsGiant(type) ? GiantDamage : BaseDamage) * Mathf.Pow(1f + DamageGrowth, level - 1) * DamageMult;
    }

    public float GetExp(int level, EnemyType type) => (IsGiant(type) ? GiantExp : BaseExp) * Mathf.Pow(1f + ExpGrowth, level - 1) * ExpMult;

    public float GetRarityBoost(EnemyType type) => IsGiant(type) ? GiantRarityBoost : 0f;

    public int GetCurrencyMax(int level, EnemyType type)
    {
        float mult = IsGiant(type) ? GiantCurrencyMult : 1f;
        float amount = BaseCurrencyMax * mult * (1f + CurrencyGrowth * (level - 1));
        return Mathf.Max(1, Mathf.RoundToInt(amount));
    }

    public int RollCurrencyAmount(int level, EnemyType type)
    {
        int max = GetCurrencyMax(level, type);
        int min = Mathf.Max(1, Mathf.RoundToInt(max * CurrencyMinPercent));

        // The int version of Random.Range excludes the max, so add 1
        return Random.Range(min, max + 1);
    }
}
