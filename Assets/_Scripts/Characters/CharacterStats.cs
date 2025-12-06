using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStats : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Health")]
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Stats")]
    public NetworkVariable<float> Speed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> Damage = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> AttackSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CoolDownReduction = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> Armor = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Events")]
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        Health.Value = Mathf.Max(Health.Value - roundedDamage, 0);

        // Feedback
        OnDamaged?.Invoke(roundedDamage);

        if (Health.Value <= 0)
        {
            // Die
        }
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType)
    {
        float armor = Armor.Value; // Get the target's current armor

        switch (damageType)
        {
            case DamageType.Flat:
                {
                    float armorMultiplier = 100f / (100f + armor); // How much of the damage is applied after armor
                    return baseDamage * armorMultiplier; // Flat base damage reduced by armor
                }

            case DamageType.Percent:
                {
                    float percentDamage = MaxHealth.Value * (baseDamage / 100f); // Calculate % of Max Health as base damage
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
            healAmount = MaxHealth.Value * (healAmount / 100f);
        }

        // Heal
        float missingHealth = MaxHealth.Value - Health.Value;
        float actualHeal = Mathf.Min(healAmount, missingHealth);
        int roundedHeal = Mathf.FloorToInt(actualHeal);

        Health.Value += roundedHeal;

        // Feedback
        OnHealed?.Invoke(roundedHeal);
    }

    public void AddModifier(StatModifier modifier)
    {
        if (!IsServer) return;

        modifiers.Add(modifier);

        switch (modifier.statType)
        {
            case StatType.Damage: IncreaseDamage(modifier.value); break;
            case StatType.Health: IncreaseHealth(modifier.value); break;
            case StatType.AttackSpeed: IncreaseAttackSpeed(modifier.value); break;
            case StatType.CoolDown: IncreaseCoolDownReduction(modifier.value); break;
        }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (!IsServer) return;
        if (modifiers.Count == 0) return;

        modifiers.Remove(modifier);

        switch (modifier.statType)
        {
            case StatType.Damage: DecreaseDamage(modifier.value); break;
            case StatType.Health: DecreaseHealth(modifier.value); break;
            case StatType.AttackSpeed: DecreaseAttackSpeed(modifier.value); break;
            case StatType.CoolDown: DecreaseCoolDownReduction(modifier.value); break;
        }
    }

    #region Damage
    public void IncreaseDamage(int amount)
    {
        if (IsServer)
        {
            Damage.Value += amount;
        }
        else
        {
            IncreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseDamageServerRPC(int amount)
    {
        Damage.Value += amount;
    }

    public void DecreaseDamage(int amount)
    {
        if (IsServer)
        {
            Damage.Value -= amount;
        }
        else
        {
            DecreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseDamageServerRPC(int amount)
    {
        Damage.Value -= amount;
    }
    #endregion

    #region Health
    public void IncreaseHealth(int amount)
    {
        if (IsServer)
        {
            Health.Value += amount;
        }
        else
        {
            IncreaseHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseHealthServerRPC(int amount)
    {
        Health.Value += amount;
    }

    public void DecreaseHealth(int amount)
    {
        if (IsServer)
        {
            Health.Value -= amount;
        }
        else
        {
            DecreaseHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseHealthServerRPC(int amount)
    {
        Health.Value -= amount;
    }
    #endregion

    #region AttackSpeed
    public void IncreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            AttackSpeed.Value += amount;
        }
        else
        {
            IncreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseAttackSpeedServerRPC(int amount)
    {
        AttackSpeed.Value += amount;
    }

    public void DecreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            AttackSpeed.Value -= amount;
        }
        else
        {
            DecreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseAttackSpeedServerRPC(int amount)
    {
        AttackSpeed.Value -= amount;
    }
    #endregion

    #region CoolDownReduction
    public void IncreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            CoolDownReduction.Value += amount;
        }
        else
        {
            IncreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseCoolDownReductionServerRPC(int amount)
    {
        CoolDownReduction.Value += amount;
    }

    public void DecreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            CoolDownReduction.Value -= amount;
        }
        else
        {
            DecreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseCoolDownReductionServerRPC(int amount)
    {
        CoolDownReduction.Value -= amount;
    }
    #endregion
}
