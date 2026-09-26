using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Character/PlayerScalingData")]
public class PlayerScalingData : ScriptableObject
{
    [Header("Level 1 Baseline")]
    public float BaseHealth;
    public float BaseHealthRegen;
    public float BaseMana;
    public float BaseManaRegen;
    public float BaseEndurance;
    public float BaseEnduranceRegen;
    public float BaseDamage;
    public float BaseAttackSpeed;
    public float BaseCDR;
    public float BaseArmor;
    public float BaseSpeed;
    public float BaseVamp;

    [Header("Flat Growth Per Level")]
    public float HealthPerLevel;
    public float DamagePerLevel;
    public float ManaPerLevel;

    [Header("Interval Growth")]
    public int RegenLevelInterval;
    public float HealthRegenPerInterval;
    public float ManaRegenPerInterval;

    [Header("Attribute Points")]
    public int AttributePointsPerLevel;

    [Header("Experience Curve")]
    [Range(1f, 300f)] public float XpAdditionMultiplier;
    [Range(2f, 4f)] public float XpPowerMultiplier;
    [Range(7f, 14f)] public float XpDivisionMultiplier;

    public int CalculateRequiredXp(int level)
    {
        int total = 0;
        for (int i = 1; i <= level; i++)
        {
            total += (int)Mathf.Floor(i + XpAdditionMultiplier * Mathf.Pow(XpPowerMultiplier, i / XpDivisionMultiplier));
        }
        return total / 4;
    }

    public void ApplyLevelUpGains(PlayerStats stats, int newLevel)
    {
        stats.IncreaseStat(StatType.Health, HealthPerLevel);
        stats.IncreaseStat(StatType.Damage, DamagePerLevel);
        stats.IncreaseStat(StatType.Mana, ManaPerLevel);

        if (newLevel % RegenLevelInterval == 0)
        {
            stats.IncreaseStat(StatType.HealthRegen, HealthRegenPerInterval);
            stats.IncreaseStat(StatType.ManaRegen, ManaRegenPerInterval);
        }
    }
}
