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

    [Header("Stats")]
    public float Speed;
    public int Damage;
    public float AttackSpeed;
    public float CoolDownReduction;
    public float Armor;
    public int TotalDamage => Damage + GetModifierInt(StatType.Damage);

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("Events")]
    [HideInInspector] public UnityEvent<float> OnDamaged;
    [HideInInspector] public UnityEvent<float> OnHealed;
    [HideInInspector] public UnityEvent<float, Vector2> OnDamageDealt;
    [HideInInspector] public UnityEvent OnDeath;

    [HideInInspector] public UnityEvent<NetworkObject> OnEnemyDamaged;
    [HideInInspector] public UnityEvent<NetworkObject> OnEnemyDeath;

    public int GetModifierInt(StatType type)
    {
        int value = 0;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type) value += mod.value;
        }

        return value;
    }

    public float GetModifierFloat(StatType type)
    {
        float value = 0;

        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type) value += mod.value;
        }

        return value;
    }

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        net_CurrentHealth.Value = Mathf.Max(net_CurrentHealth.Value - roundedDamage, 0);

        // Feedback
        OnDamaged?.Invoke(roundedDamage);
        OnEnemyDamaged?.Invoke(attackerID);

        if (net_CurrentHealth.Value <= 0)
        {
            ClearTarget(attackerID);
            OnDeath?.Invoke();
            OnEnemyDeath?.Invoke(attackerID);
        }
    }

    void ClearTarget(NetworkObject attackerID)
    {
        EnemyStateMachine enemy = attackerID.GetComponent<EnemyStateMachine>();
        if (enemy != null) enemy.Target = null;

        NPCStateMachine npc = attackerID.GetComponent<NPCStateMachine>();
        if (npc != null) npc.Target = null;
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
                    float percentDamage = net_TotalHealth.Value * (baseDamage / 100f); // Calculate % of Max Health as base damage
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
            healAmount = net_TotalHealth.Value * (healAmount / 100f);
        }

        // Heal
        float missingHealth = net_TotalHealth.Value - net_CurrentHealth.Value;
        float actualHeal = Mathf.Min(healAmount, missingHealth);
        int roundedHeal = Mathf.FloorToInt(actualHeal);

        net_CurrentHealth.Value += roundedHeal;

        // Feedback
        OnHealed?.Invoke(roundedHeal);
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);

        if (modifier.statType == StatType.Health)
        {
            if (IsServer)
            {
                net_CurrentHealth.Value += modifier.value;
                RecalculateTotalHealth();
            }
            else
            {
                HPIncreaseServerRPC(modifier.value);
            }

        }
    }

    [ServerRpc]
    void HPIncreaseServerRPC(float value)
    {
        net_CurrentHealth.Value += value;
        RecalculateTotalHealth();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.Count == 0) return;

        modifiers.Remove(modifier);

        if (modifier.statType == StatType.Health)
        {
            if (IsServer)
            {
                net_CurrentHealth.Value -= modifier.value;
                RecalculateTotalHealth();
            }
            else
            {
                HPDecreaseServerRPC(modifier.value);
            }

        }
    }

    [ServerRpc]
    void HPDecreaseServerRPC(float value)
    {
        net_CurrentHealth.Value -= value;
        RecalculateTotalHealth();
    }

    public void RecalculateTotalHealth()
    {
        float healthFromModifiers = GetModifierFloat(StatType.Health);

        if (IsServer)
        {
            net_TotalHealth.Value = net_BaseHealth.Value + healthFromModifiers;
        }
        else
        {
            RecalculateTotalHealthServerRPC(healthFromModifiers);
        }
    }

    [ServerRpc]
    void RecalculateTotalHealthServerRPC(float healthFromModifiers)
    {
        net_TotalHealth.Value = net_BaseHealth.Value + healthFromModifiers;
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
            net_BaseHealth.Value += amount;
            net_CurrentHealth.Value += amount;
            RecalculateTotalHealth();
        }
        else
        {
            IncreaseHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseHealthServerRPC(int amount)
    {
        net_BaseHealth.Value += amount;
        net_CurrentHealth.Value += amount;
        RecalculateTotalHealth();
    }

    public void DecreaseHealth(int amount)
    {
        if (IsServer)
        {
            net_BaseHealth.Value -= amount;
            net_CurrentHealth.Value -= amount;
            RecalculateTotalHealth();
        }
        else
        {
            DecreaseHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseHealthServerRPC(int amount)
    {
        net_BaseHealth.Value -= amount;
        net_CurrentHealth.Value -= amount;
        RecalculateTotalHealth();
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
