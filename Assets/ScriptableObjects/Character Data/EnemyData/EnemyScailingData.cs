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

    [Header("Giant Drops")]
    [Tooltip("Added to the rarity decay factor when rolling drops. Higher = better chance at rare items.")]
    [Range(0f, 0.5f)] public float GiantRarityBoost = 0.15f;

    public float GetRarityBoost(EnemyType type) => IsGiant(type) ? GiantRarityBoost : 0f;
}
