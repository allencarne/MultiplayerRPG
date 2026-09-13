using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Health")]
    public NetworkVariable<float> net_BaseHP = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_CurrentHP = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_TotalHP = new(writePerm: NetworkVariableWritePermission.Server);
    public bool isDead;

    [Header("Base Stats")]
    public NetworkVariable<float> net_BaseDamage = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseArmor = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseAS = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseCDR = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseVamp = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_BaseHealthRegen = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Total Stats")]
    public float TotalDamage => (net_BaseDamage.Value + GetModifier(StatType.Damage)) * (1f + GetPercentModifier(StatType.Damage));
    public float TotalArmor => net_BaseArmor.Value + GetModifier(StatType.Armor);
    public float TotalAS => (net_BaseAS.Value + GetModifier(StatType.AttackSpeed)) * (1f + GetPercentModifier(StatType.AttackSpeed));
    public float TotalCDR => (net_BaseCDR.Value + GetModifier(StatType.CoolDown)) * (1f + GetPercentModifier(StatType.CoolDown));
    public float TotalSpeed => Mathf.Max((net_BaseSpeed.Value + GetModifier(StatType.Speed)) * (1f + GetPercentModifier(StatType.Speed)), minSpeed);
    public float TotalVamp => (net_BaseVamp.Value + GetModifier(StatType.Vamp)) * (1f + GetPercentModifier(StatType.Vamp));
    public float TotalHealthRegen => (net_BaseHealthRegen.Value + GetModifier(StatType.HealthRegen)) * (1f + GetPercentModifier(StatType.HealthRegen));

    float minSpeed = .2f;

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("Events")]
    [HideInInspector] public UnityEvent<float> OnDamaged;
    [HideInInspector] public UnityEvent<float> OnHealed;
    [HideInInspector] public UnityEvent OnDamageDealt;

    [HideInInspector] public UnityEvent<NetworkObject> OnCharacterDamaged;
    [HideInInspector] public UnityEvent<NetworkObject> OnCharacterDeath;

    [HideInInspector] public UnityEvent OnDeath;
    [HideInInspector] public UnityEvent OnInterrupted;

    public float TakeDamage(float damage, DamageType damageType, NetworkObject attackerID, Vector2 position)
    {
        if (!IsServer) return 0f;
        if (isDead) return 0f;

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        net_CurrentHP.Value = Mathf.Max(net_CurrentHP.Value - roundedDamage, 0);

        // Feedback
        OnDamaged?.Invoke(roundedDamage);
        OnCharacterDamaged?.Invoke(attackerID);

        CharacterStats attackerStats = attackerID.GetComponent<CharacterStats>();
        if (attackerStats != null)
        {
            attackerStats.OnDamageDealt?.Invoke();
        }

        if (net_CurrentHP.Value <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
            OnCharacterDeath?.Invoke(attackerID);
        }

        return roundedDamage;
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType)
    {
        float armor = TotalArmor;

        switch (damageType)
        {
            case DamageType.Flat:
                {
                    float armorMultiplier = 100f / (100f + armor); // How much of the damage is applied after armor
                    return baseDamage * armorMultiplier; // Flat base damage reduced by armor
                }

            case DamageType.Percent:
                {
                    float percentDamage = net_TotalHP.Value * (baseDamage / 100f); // Calculate % of Max Health as base damage
                    float armorMultiplier = 100f / (100f + armor); // Still apply armor reduction
                    return percentDamage * armorMultiplier; // % Health damage reduced by armor
                }

            case DamageType.True:
                {
                    return baseDamage; // Ignore Armor
                }

            default:
                {
                    return baseDamage; // Fallback
                }
        }
    }

    public void GiveHeal(float healAmount, HealType healType)
    {
        if (!IsServer) return;

        if (healType == HealType.Percentage)
        {
            healAmount = net_TotalHP.Value * (healAmount / 100f);
        }

        // Heal
        float missingHealth = net_TotalHP.Value - net_CurrentHP.Value;
        float actualHeal = Mathf.Min(healAmount, missingHealth);
        int roundedHeal = Mathf.FloorToInt(actualHeal);

        net_CurrentHP.Value += roundedHeal;

        // Feedback
        OnHealed?.Invoke(roundedHeal);
    }

    #region Modifiers

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        float modHealth = GetModifier(StatType.Health);


        if (modifier.statType == StatType.Health)
        {
            // Keep Health modifier behavior flat-only to avoid changing HP RPCs.
            if (modifier.modType == ModType.Flat)
            {
                if (IsServer)
                {
                    net_CurrentHP.Value += modifier.value;
                    RecalculateTotalHealth(modHealth);
                }
                else
                {
                    HPIncreaseServerRPC(modifier.value);
                }
            }
            else
            {
                // If a percent health modifier is ever introduced, handle it here (not currently produced by rules).
            }
        }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.Count == 0) return;

        modifiers.Remove(modifier);

        if (modifier.statType == StatType.Health)
        {
            if (IsServer)
            {
                net_CurrentHP.Value -= modifier.value;
                RecalculateTotalHealth(GetModifier(StatType.Health));
            }
            else
            {
                HPDecreaseServerRPC(modifier.value);
            }

        }
    }

    public float GetModifier(StatType type, ModSource? source = null)
    {
        float value = 0;
        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type)
            {
                if (source == null || mod.source == source)
                {
                    // Only count flat modifiers here (preserve existing behavior)
                    if (mod.modType == ModType.Flat) value += mod.value;
                }
            }
        }
        return value;
    }

    public float GetPercentModifier(StatType type, ModSource? source = null)
    {
        float value = 0f;
        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type)
            {
                if (source == null || mod.source == source)
                {
                    if (mod.modType == ModType.Percent) value += mod.value;
                }
            }
        }
        return value;
    }

    #endregion

    #region Stats

    public void IncreaseStat(StatType stat, float amount) => ModifyBaseStat(stat, Mathf.Abs(amount));
    public void DecreaseStat(StatType stat, float amount) => ModifyBaseStat(stat, -Mathf.Abs(amount));

    public void ModifyBaseStat(StatType stat, float amount)
    {
        if (IsServer)
        {
            ApplyStatChange(stat, amount);
        }
        else
        {
            ModifyBaseStatServerRPC(stat, amount);
        }
    }

    [ServerRpc]
    void ModifyBaseStatServerRPC(StatType stat, float amount)
    {
        ApplyStatChange(stat, amount);
    }

    protected virtual void ApplyStatChange(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.Damage: net_BaseDamage.Value += amount; break;
            case StatType.AttackSpeed: net_BaseAS.Value += amount; break;
            case StatType.CoolDown: net_BaseCDR.Value += amount; break;
            case StatType.Speed: net_BaseSpeed.Value += amount; break;
            case StatType.Armor: net_BaseArmor.Value += amount; break;
            case StatType.Vamp: net_BaseVamp.Value += amount; break;
            case StatType.HealthRegen: net_BaseHealthRegen.Value += amount; break;

            case StatType.Health:
                net_BaseHP.Value += amount;
                net_CurrentHP.Value += amount;
                RecalculateTotalHealth(GetModifier(StatType.Health));
                break;

            default:
                // Reaches here for stats this class doesn't know about (e.g. Mana/Endurance
                // on a non-player CharacterStats). Safe no-op, but flagged so a bad call
                // doesn't fail silently forever.
                Debug.LogWarning($"{GetType().Name} has no handling for {stat}.");
                break;
        }
    }

    #endregion

    #region Health

    [ServerRpc]
    void HPIncreaseServerRPC(float value)
    {
        float modHealth = GetModifier(StatType.Health);
        net_CurrentHP.Value += value;
        RecalculateTotalHealth(modHealth);
    }

    [ServerRpc]
    void HPDecreaseServerRPC(float value)
    {
        float modHealth = GetModifier(StatType.Health);
        net_CurrentHP.Value -= value;
        net_TotalHP.Value = net_BaseHP.Value + modHealth;
    }

    public void RecalculateTotalHealth(float modHealth)
    {
        if (!IsServer) return;
        net_TotalHP.Value = net_BaseHP.Value + modHealth;
    }

    #endregion
}
