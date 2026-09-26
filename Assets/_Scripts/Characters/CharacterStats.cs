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

    public float TakeDamage(float damage, DamageType damageType, NetworkObject attackerID, Vector2 position)
    {
        // Return if not server or dead
        if (!IsServer) return 0f;
        if (isDead) return 0f;

        // Don't take Damage if Immune
        Buffs buffs = GetComponent<Buffs>();
        if (buffs != null && buffs.immune != null && buffs.immune.net_IsImmune.Value) return 0f;

        // Calculate the amount of damage that should actually be dealt after armor and damage type are considered.
        float finalDamage = CalculateFinalDamage(damage, damageType);

        // Round the calculated damage to the nearest whole number.
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract the final damage from the character's current health, but never allow health to go below zero.
        net_CurrentHP.Value = Mathf.Max(net_CurrentHP.Value - roundedDamage, 0);

        // Tell anything listening that this character took damage and provide the damage amount.
        OnDamaged?.Invoke(roundedDamage);

        // Tell anything listening that this character was damaged and provide the attacking NetworkObject.
        OnCharacterDamaged?.Invoke(attackerID);

        // Try to find CharacterStats on the object that dealt the damage.
        CharacterStats attackerStats = attackerID.GetComponent<CharacterStats>();

        // Tell the attacker that they successfully dealt damage.
        if (attackerStats != null) attackerStats.OnDamageDealt?.Invoke();

        // Check whether the character's health has reached zero.
        if (net_CurrentHP.Value <= 0)
        {
            // Mark the character as dead.
            isDead = true;

            // Tell anything listening that this character has died.
            OnDeath?.Invoke();

            // Tell anything listening that this character has died and provide the attacker.
            OnCharacterDeath?.Invoke(attackerID);
        }

        // Return the amount of damage that was actually dealt.
        return roundedDamage;
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType)
    {
        float armor = TotalArmor;
        float armorMultiplier = 100f / (100f + armor);

        switch (damageType)
        {
            case DamageType.Flat:
                return baseDamage * armorMultiplier;

            case DamageType.True:
                return baseDamage;

            case DamageType.PercentMaxHealth:
                return net_TotalHP.Value * (baseDamage / 100f) * armorMultiplier;

            case DamageType.PercentMaxHealthTrue:
                return net_TotalHP.Value * (baseDamage / 100f);

            case DamageType.PercentMissingHealth:
                {
                    float missing = net_TotalHP.Value - net_CurrentHP.Value;
                    return missing * (baseDamage / 100f) * armorMultiplier;
                }

            case DamageType.PercentMissingHealthTrue:
                {
                    float missing = net_TotalHP.Value - net_CurrentHP.Value;
                    return missing * (baseDamage / 100f);
                }

            case DamageType.PercentCurrentHealth:
                return net_CurrentHP.Value * (baseDamage / 100f) * armorMultiplier;

            case DamageType.PercentCurrentHealthTrue:
                return net_CurrentHP.Value * (baseDamage / 100f);

            default:
                return baseDamage;
        }
    }

    public void GiveHeal(float healAmount, HealType healType)
    {
        // Only the server is allowed to modify health.
        if (!IsServer) return;
        if (isDead) return;

        // Check whether the heal should be calculated as a percentage.
        if (healType == HealType.Percentage)
        {
            // Convert the percentage into an actual amount based on maximum health.
            healAmount = net_TotalHP.Value * (healAmount / 100f);
        }

        // Calculate how much health the character is currently missing.
        float missingHealth = net_TotalHP.Value - net_CurrentHP.Value;

        // Make sure the heal cannot restore more health than the character is missing.
        float actualHeal = Mathf.Min(healAmount, missingHealth);

        // Round the final healing amount to the nearest whole number.
        int roundedHeal = Mathf.RoundToInt(actualHeal);

        // Add the healing amount to the character's current health.
        net_CurrentHP.Value += roundedHeal;

        // Tell anything listening that the character was healed and provide the amount healed.
        OnHealed?.Invoke(roundedHeal);
    }

    #region Modifiers

    public void AddModifier(StatModifier modifier)
    {
        // Add the supplied modifier to the character's modifier list.
        modifiers.Add(modifier);

        // Check whether this is a flat Health modifier.
        if (modifier.statType == StatType.Health && modifier.modType == ModType.Flat)
        {
            // Increase current health by the amount of the new Health modifier.
            ChangeCurrentHealth(modifier.value);
        }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        // Return if there are no modifiers to remove.
        if (modifiers.Count == 0) return;

        // Remove the supplied modifier from the character's modifier list.
        modifiers.Remove(modifier);

        // Check whether this is a flat Health modifier.
        if (modifier.statType == StatType.Health && modifier.modType == ModType.Flat)
        {
            // Decrease current health by the amount of the removed Health modifier.
            ChangeCurrentHealth(-modifier.value);
        }
    }

    public float GetModifier(StatType type, ModSource? source = null)
    {
        // Start with a total modifier value of zero.
        float value = 0;

        // Go through every modifier currently affecting this character.
        foreach (StatModifier mod in modifiers)
        {
            // Check whether this modifier affects the stat we are looking for.
            if (mod.statType == type)
            {
                // Check whether a source was specified, or whether we accept modifiers from any source.
                if (source == null || mod.source == source)
                {
                    // Only add flat modifiers to this calculation.
                    if (mod.modType == ModType.Flat) value += mod.value;
                }
            }
        }

        // Return the combined flat modifier value.
        return value;
    }

    public float GetPercentModifier(StatType type, ModSource? source = null)
    {
        // Start with a total percentage modifier value of zero.
        float value = 0f;

        // Go through every modifier currently affecting this character.
        foreach (StatModifier mod in modifiers)
        {
            // Check whether this modifier affects the stat we are looking for.
            if (mod.statType == type)
            {
                // Check whether a source was specified, or whether we accept modifiers from any source.
                if (source == null || mod.source == source)
                {
                    // Only add percentage modifiers to this calculation.
                    if (mod.modType == ModType.Percent) value += mod.value;
                }
            }
        }

        // Return the combined percentage modifier value.
        return value;
    }

    #endregion

    #region Stats

    public void IncreaseStat(StatType stat, float amount) => ModifyBaseStat(stat, Mathf.Abs(amount));
    public void DecreaseStat(StatType stat, float amount) => ModifyBaseStat(stat, -Mathf.Abs(amount));

    public void ModifyBaseStat(StatType stat, float amount)
    {
        // Check whether this code is currently running on the server.
        if (IsServer)
        {
            // The server can directly apply the stat change.
            ApplyStatChange(stat, amount);
        }
        else
        {
            // Ask the server to apply the stat change because clients cannot directly modify these NetworkVariables.
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

            default: Debug.LogWarning($"{GetType().Name} has no handling for {stat}."); break;
        }
    }

    #endregion

    #region Health

    void ChangeCurrentHealth(float amount)
    {
        // Check whether this code is running on the server.
        if (IsServer)
        {
            // The server can directly change current health.
            ApplyCurrentHealthChange(amount);
        }
        else
        {
            // Ask the server to change current health because the NetworkVariable is server-write-only.
            ChangeCurrentHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void ChangeCurrentHealthServerRPC(float amount)
    {
        ApplyCurrentHealthChange(amount);
    }

    void ApplyCurrentHealthChange(float amount)
    {
        // Change the character's current health by the supplied amount.
        net_CurrentHP.Value += amount;

        // Recalculate maximum health because a Health modifier may have changed.
        RecalculateTotalHealth(GetModifier(StatType.Health));
    }

    public void RecalculateTotalHealth(float modHealth)
    {
        // Only the server is allowed to update the total health NetworkVariable.
        if (!IsServer) return;

        // Calculate maximum health by adding base health and all flat Health modifiers.
        net_TotalHP.Value = net_BaseHP.Value + modHealth;
    }

    #endregion
}
