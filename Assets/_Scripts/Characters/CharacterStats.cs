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
    public float Speed;
    public int Damage;
    public float AttackSpeed;
    public float CoolDownReduction;
    public float Armor;

    public int ModifiedDamage => Damage + GetModifierInt(StatType.Damage);
    public float ModifiedMaxHealth => MaxHealth.Value + GetModifierFloat(StatType.Health);

    int GetModifierInt(StatType type)
    {
        int value = 0;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type)
            {
                value += mod.value;
            }
        }

        return value;
    }

    float GetModifierFloat(StatType type)
    {
        float value = 0;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type)
            {
                value += mod.value;
            }
        }

        return value;
    }

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("Events")]
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

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
        float armor = Armor; // Get the target's current armor

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
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (!IsServer) return;
        if (modifiers.Count == 0) return;

        modifiers.Remove(modifier);
    }

    #region Damage
    public void IncreaseDamage(int amount)
    {
        if (IsServer)
        {
            Damage += amount;
        }
        else
        {
            IncreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseDamageServerRPC(int amount)
    {
        Damage += amount;
    }

    public void DecreaseDamage(int amount)
    {
        if (IsServer)
        {
            Damage -= amount;
        }
        else
        {
            DecreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseDamageServerRPC(int amount)
    {
        Damage -= amount;
    }
    #endregion

    #region Health
    public void IncreaseHealth(int amount)
    {
        if (IsServer)
        {
            MaxHealth.Value += amount;
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
        MaxHealth.Value += amount;
        Health.Value += amount;
    }

    public void DecreaseHealth(int amount)
    {
        if (IsServer)
        {
            MaxHealth.Value -= amount;
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
        MaxHealth.Value -= amount;
        Health.Value -= amount;
    }
    #endregion

    #region AttackSpeed
    public void IncreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            AttackSpeed += amount;
        }
        else
        {
            IncreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseAttackSpeedServerRPC(int amount)
    {
        AttackSpeed += amount;
    }

    public void DecreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            AttackSpeed -= amount;
        }
        else
        {
            DecreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseAttackSpeedServerRPC(int amount)
    {
        AttackSpeed -= amount;
    }
    #endregion

    #region CoolDownReduction
    public void IncreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            CoolDownReduction += amount;
        }
        else
        {
            IncreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseCoolDownReductionServerRPC(int amount)
    {
        CoolDownReduction += amount;
    }

    public void DecreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            CoolDownReduction -= amount;
        }
        else
        {
            DecreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseCoolDownReductionServerRPC(int amount)
    {
        CoolDownReduction -= amount;
    }
    #endregion
}
