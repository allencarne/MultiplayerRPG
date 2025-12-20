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

    [Header("Stats")]
    public float BaseArmor;

    public float BaseDamage;
    public float BaseAS;
    public float BaseCDR;
    public float BaseSpeed;
    public float TotalDamage => BaseDamage + GetModifier(StatType.Damage);
    public float TotalAS => BaseAS + GetModifier(StatType.AttackSpeed);
    public float TotalCDR => BaseCDR + GetModifier(StatType.CoolDown);
    public float TotalSpeed => Mathf.Max(BaseSpeed + GetModifier(StatType.Speed), minSpeed);

    float minSpeed = .2f;

    [Header("List")]
    public List<StatModifier> modifiers = new List<StatModifier>();

    [Header("Events")]
    [HideInInspector] public UnityEvent<float> OnDamaged;
    [HideInInspector] public UnityEvent<float> OnHealed;
    [HideInInspector] public UnityEvent<float, Vector2> OnDamageDealt;
    [HideInInspector] public UnityEvent OnDeath;

    [HideInInspector] public UnityEvent<NetworkObject> OnEnemyDamaged;
    [HideInInspector] public UnityEvent<NetworkObject> OnEnemyDeath;

    public float GetModifier(StatType type, ModSource? source = null)
    {
        float value = 0;
        foreach (StatModifier mod in modifiers)
        {
            if (mod.statType == type)
            {
                if (source == null || mod.source == source)
                {
                    value += mod.value;
                }
            }
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
        net_CurrentHP.Value = Mathf.Max(net_CurrentHP.Value - roundedDamage, 0);

        // Feedback
        OnDamaged?.Invoke(roundedDamage);
        OnEnemyDamaged?.Invoke(attackerID);

        if (net_CurrentHP.Value <= 0)
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
        float armor = BaseArmor; // Get the target's current armor

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

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        float modHealth = GetModifier(StatType.Health);


        if (modifier.statType == StatType.Health)
        {
            if (IsServer)
            {
                net_CurrentHP.Value += modifier.value;
                RecalculateTotalHealth(modHealth);
            }
            else
            {
                HPIncreaseServerRPC(modifier.value, modHealth);
            }
        }
    }

    [ServerRpc]
    void HPIncreaseServerRPC(float value, float modHealth)
    {
        net_CurrentHP.Value += value;
        RecalculateTotalHealth(modHealth);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.Count == 0) return;

        modifiers.Remove(modifier);
        float modHealth = GetModifier(StatType.Health);

        if (modifier.statType == StatType.Health)
        {
            if (IsServer)
            {
                net_CurrentHP.Value -= modifier.value;
                RecalculateTotalHealth(modHealth);
            }
            else
            {
                HPDecreaseServerRPC(modifier.value, modHealth);
            }

        }
    }

    [ServerRpc]
    void HPDecreaseServerRPC(float value, float modHealth)
    {
        net_CurrentHP.Value -= value;
        RecalculateTotalHealth(modHealth);
    }

    public void RecalculateTotalHealth(float modHealth)
    {
        if (!IsServer) return;
        net_TotalHP.Value = net_BaseHP.Value + modHealth;
    }

    #region Damage
    public void IncreaseDamage(int amount)
    {
        if (IsServer)
        {
            BaseDamage += amount;
        }
        else
        {
            IncreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseDamageServerRPC(int amount)
    {
        BaseDamage += amount;
    }

    public void DecreaseDamage(int amount)
    {
        if (IsServer)
        {
            BaseDamage -= amount;
        }
        else
        {
            DecreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseDamageServerRPC(int amount)
    {
        BaseDamage -= amount;
    }
    #endregion

    #region Health
    public void IncreaseHealth(int amount)
    {
        float modHealth = GetModifier(StatType.Health);

        if (IsServer)
        {
            net_BaseHP.Value += amount;
            net_CurrentHP.Value += amount;
            RecalculateTotalHealth(modHealth);
        }
        else
        {
            IncreaseHealthServerRPC(amount, modHealth);
        }
    }

    [ServerRpc]
    void IncreaseHealthServerRPC(int amount, float modHealth)
    {
        net_BaseHP.Value += amount;
        net_CurrentHP.Value += amount;
        RecalculateTotalHealth(modHealth);
    }

    public void DecreaseHealth(int amount)
    {
        float modHealth = GetModifier(StatType.Health);

        if (IsServer)
        {
            net_BaseHP.Value -= amount;
            net_CurrentHP.Value -= amount;
            RecalculateTotalHealth(modHealth);
        }
        else
        {
            DecreaseHealthServerRPC(amount, modHealth);
        }
    }

    [ServerRpc]
    void DecreaseHealthServerRPC(int amount, float modHealth)
    {
        net_BaseHP.Value -= amount;
        net_CurrentHP.Value -= amount;
        RecalculateTotalHealth(modHealth);
    }
    #endregion

    #region AttackSpeed
    public void IncreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            BaseAS += amount;
        }
        else
        {
            IncreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseAttackSpeedServerRPC(int amount)
    {
        BaseAS += amount;
    }

    public void DecreaseAttackSpeed(int amount)
    {
        if (IsServer)
        {
            BaseAS -= amount;
        }
        else
        {
            DecreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseAttackSpeedServerRPC(int amount)
    {
        BaseAS -= amount;
    }
    #endregion

    #region CoolDownReduction
    public void IncreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            BaseCDR += amount;
        }
        else
        {
            IncreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseCoolDownReductionServerRPC(int amount)
    {
        BaseCDR += amount;
    }

    public void DecreaseCoolDownReduction(int amount)
    {
        if (IsServer)
        {
            BaseCDR -= amount;
        }
        else
        {
            DecreaseCoolDownReductionServerRPC(amount);
        }
    }

    [ServerRpc]
    void DecreaseCoolDownReductionServerRPC(int amount)
    {
        BaseCDR -= amount;
    }
    #endregion
}
