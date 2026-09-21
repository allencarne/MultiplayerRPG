using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScailingData", menuName = "Scriptable Objects/Character/EnemyScailingData")]
public class EnemyScailingData : ScriptableObject
{
    [Header("Level 1 Baseline")]
    public float BaseHealth = 30f;
    public float BaseDamage = 12f;
    public float BaseExp = 1f;

    [Header("Giant Level 1 Baseline")]
    public float GiantHealth = 300f;
    public float GiantDamage = 18f;
    public float GiantExp = 10f;
    public float GiantArmorMult = 1.5f;

    [Header("Dummy")]
    public float DummyHealth = 999f;

    [Header("Growth")]
    public float HealthGrowth = 0.10f;
    public float DamageGrowth = 0.06f;
    public float ExpGrowth = 0.08f;
    public float ArmorPerLevel = 1.5f;

    [Header("Multipliers")]
    public float HealthMult = 1f;
    public float DamageMult = 1f;
    public float ArmorMult = 1f;
    public float ExpMult = 1f;

    [Header("Giant Drops")]
    [Tooltip("Added to the rarity decay factor when rolling drops. Higher = better chance at rare items.")]
    [Range(0f, 0.5f)] public float GiantRarityBoost = 0.15f;

    [Header("Currency / Collectable Drops")]
    [Tooltip("Max amount a regular enemy drops at level 1")]
    public int BaseCurrencyMax = 3;
    [Tooltip("Giants drop this many times more than a regular enemy")]
    public float GiantCurrencyMult = 5f;
    [Tooltip("Extra % per level (0.15 = +15% per level, same as your chests)")]
    public float CurrencyGrowth = 0.15f;
    [Tooltip("The minimum drop is this % of the max, so higher levels aren't 1-to-huge")]
    [Range(0f, 1f)] public float CurrencyMinPercent = 0.33f;

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
