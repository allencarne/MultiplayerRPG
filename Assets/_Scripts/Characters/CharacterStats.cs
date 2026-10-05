using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Health")]
    public NetworkVariable<float> net_BaseHealth = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_CurrentHealth = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_TotalHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Health Regen")]
    public NetworkVariable<float> net_BaseHealthRegen = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalHealthRegen => (net_BaseHealthRegen.Value + GetModifier(StatType.HealthRegen)) * (1f + GetPercentModifier(StatType.HealthRegen));

    [Header("Recharge")]
    public NetworkVariable<float> net_BaseCDR = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalCDR => (net_BaseCDR.Value + GetModifier(StatType.Recharge)) * (1f + GetPercentModifier(StatType.Recharge));

    [Header("Dexterity")]
    public NetworkVariable<float> net_BaseAS = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalAS => (net_BaseAS.Value + GetModifier(StatType.Dexterity)) * (1f + GetPercentModifier(StatType.Dexterity));

    [Header("Precision")]
    public NetworkVariable<float> net_BasePrecision = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalPrecision => (net_BasePrecision.Value + GetModifier(StatType.Precision)) * (1f + GetPercentModifier(StatType.Precision));

    [Header("Ferocity")]
    public NetworkVariable<float> net_BaseFerocity = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalFerocity => (net_BaseFerocity.Value + GetModifier(StatType.Ferocity)) * (1f + GetPercentModifier(StatType.Ferocity));

    [Header("Power")]
    public NetworkVariable<float> net_BaseDamage = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalDamage => (net_BaseDamage.Value + GetModifier(StatType.Power)) * (1f + GetPercentModifier(StatType.Power));

    [Header("Vamp")]
    public NetworkVariable<float> net_BaseVamp = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalVamp => (net_BaseVamp.Value + GetModifier(StatType.Vamp)) * (1f + GetPercentModifier(StatType.Vamp));

    [Header("Armor")]
    public NetworkVariable<float> net_BaseArmor = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalArmor => net_BaseArmor.Value + GetModifier(StatType.Armor);

    [Header("Lethality")]
    public NetworkVariable<float> net_BaseLethality = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalLethality => (net_BaseLethality.Value + GetModifier(StatType.Lethality)) * (1f + GetPercentModifier(StatType.Lethality));

    [Header("Speed")]
    public NetworkVariable<float> net_BaseSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalSpeed => Mathf.Max((net_BaseSpeed.Value + GetModifier(StatType.Speed)) * (1f + GetPercentModifier(StatType.Speed)), minSpeed);

    [Header("Variables")]
    public bool isDead;
    public float baseCritBonus = 0.5f;
    float minSpeed = .2f;

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("Events")]
    [HideInInspector] public UnityEvent<float> OnDamaged;
    [HideInInspector] public UnityEvent<int, HitType> OnHitTaken;
    [HideInInspector] public UnityEvent<float> OnHealed;
    [HideInInspector] public UnityEvent OnDamageDealt;
    [HideInInspector] public UnityEvent<NetworkObject> OnCharacterDamaged;
    [HideInInspector] public UnityEvent<NetworkObject> OnCharacterDeath;
    [HideInInspector] public UnityEvent OnDeath;
    [HideInInspector] public UnityEvent<int, NetworkObject> OnCritDealt;
    [HideInInspector] public UnityEvent<int, NetworkObject> OnCritTaken;

    public int TakeDamage(float damage, DamageType damageType, NetworkObject attackerID, Vector2 position, HitType hitType = HitType.Normal)
    {
        // Return if not server or dead
        if (!IsServer) return 0;
        if (isDead) return 0;

        // Don't take Damage if Immune
        Buffs buffs = GetComponent<Buffs>();
        if (buffs != null && buffs.immune != null && buffs.immune.net_IsImmune.Value) return 0;

        // Try to find CharacterStats on the object that dealt the damage
        CharacterStats attackerStats = null;
        if (attackerID != null) attackerStats = attackerID.GetComponent<CharacterStats>();

        // Get the attacker's Lethality
        float attackerLethality = attackerStats != null ? attackerStats.TotalLethality : 0f;

        // Calculate the amount of damage that should actually be dealt after armor and damage type are considered.
        float finalDamage = CalculateFinalDamage(damage, damageType, attackerLethality);

        // Round the calculated damage to the nearest whole number.
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract the final damage from the character's current health, but never allow health to go below zero.
        net_CurrentHealth.Value = Mathf.Max(net_CurrentHealth.Value - roundedDamage, 0);

        // Damaged/Dealt Events
        OnDamaged?.Invoke(roundedDamage);
        OnHitTaken?.Invoke(roundedDamage, hitType);
        OnCharacterDamaged?.Invoke(attackerID);
        if (attackerStats != null) attackerStats.OnDamageDealt?.Invoke();

        // Check whether the character's health has reached zero.
        if (net_CurrentHealth.Value <= 0)
        {
            // Mark the character as dead.
            isDead = true;

            // Death Events
            OnDeath?.Invoke();
            OnCharacterDeath?.Invoke(attackerID);
        }

        // Return the amount of damage that was actually dealt.
        return roundedDamage;
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType, float attackerLethality = 0f)
    {
        // Apply attacker's flat lethality as armor penetration (subtract from target armor)
        float armor = TotalArmor - attackerLethality;

        // Clamp armor to a minimum of -99 to prevent division by zero or negative damage multipliers.
        if (armor <= -99f) armor = -99f;

        // Calculate the damage multiplier based on the target's armor
        float armorMultiplier = 100f / (100f + armor);

        switch (damageType)
        {
            case DamageType.Flat: return baseDamage * armorMultiplier;
            case DamageType.True: return baseDamage;
            case DamageType.PercentMaxHealth: return net_TotalHealth.Value * (baseDamage / 100f) * armorMultiplier;
            case DamageType.PercentMaxHealthTrue: return net_TotalHealth.Value * (baseDamage / 100f);

            case DamageType.PercentMissingHealth:
                {
                    float missing = net_TotalHealth.Value - net_CurrentHealth.Value;
                    return missing * (baseDamage / 100f) * armorMultiplier;
                }

            case DamageType.PercentMissingHealthTrue:
                {
                    float missing = net_TotalHealth.Value - net_CurrentHealth.Value;
                    return missing * (baseDamage / 100f);
                }

            case DamageType.PercentCurrentHealth: return net_CurrentHealth.Value * (baseDamage / 100f) * armorMultiplier;
            case DamageType.PercentCurrentHealthTrue: return net_CurrentHealth.Value * (baseDamage / 100f);
            default: return baseDamage;
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
            healAmount = net_TotalHealth.Value * (healAmount / 100f);
        }

        // Calculate how much health the character is currently missing.
        float missingHealth = net_TotalHealth.Value - net_CurrentHealth.Value;

        // Make sure the heal cannot restore more health than the character is missing.
        float actualHeal = Mathf.Min(healAmount, missingHealth);

        // Round the final healing amount to the nearest whole number.
        int roundedHeal = Mathf.RoundToInt(actualHeal);

        // Add the healing amount to the character's current health.
        net_CurrentHealth.Value += roundedHeal;

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
            case StatType.Power: net_BaseDamage.Value += amount; break;
            case StatType.Dexterity: net_BaseAS.Value += amount; break;
            case StatType.Recharge: net_BaseCDR.Value += amount; break;
            case StatType.Speed: net_BaseSpeed.Value += amount; break;
            case StatType.Armor: net_BaseArmor.Value += amount; break;
            case StatType.Vamp: net_BaseVamp.Value += amount; break;
            case StatType.HealthRegen: net_BaseHealthRegen.Value += amount; break;

            case StatType.Health:
                net_BaseHealth.Value += amount;
                net_CurrentHealth.Value += amount;
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
        net_CurrentHealth.Value += amount;

        // Recalculate maximum health because a Health modifier may have changed.
        RecalculateTotalHealth(GetModifier(StatType.Health));
    }

    public void RecalculateTotalHealth(float modHealth)
    {
        // Only the server is allowed to update the total health NetworkVariable.
        if (!IsServer) return;

        // Calculate maximum health by adding base health and all flat Health modifiers.
        net_TotalHealth.Value = net_BaseHealth.Value + modHealth;
    }

    #endregion
}
