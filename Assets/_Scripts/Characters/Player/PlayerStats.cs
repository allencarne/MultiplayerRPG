using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : CharacterStats
{
    public PlayerScailingData ScalingData;

    [Header("Customization")]
    public NetworkVariable<int> net_CharacterSlot = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString32Bytes> net_playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> net_bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> net_hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> net_eyeColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Server);

    [Header("Player Stats")]
    public NetworkVariable<int> PlayerLevel = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentExperience = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> RequiredExperience = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> AttributePoints = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Endurance")]
    public NetworkVariable<float> net_BaseEndurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_CurrentEndurance = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalEndurance =>(net_BaseEndurance.Value + GetModifier(StatType.Endurance))* (1f + GetPercentModifier(StatType.Endurance));

    [Header("Endurance Regen")]
    public NetworkVariable<float> net_BaseEnduranceRegen = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalEnduranceRegen => (net_BaseEnduranceRegen.Value + GetModifier(StatType.EnduranceRegen)) * (1f + GetPercentModifier(StatType.EnduranceRegen));

    [Header("Mana")]
    public NetworkVariable<float> net_BaseMana = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> net_CurrentMana = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalMana => (net_BaseMana.Value + GetModifier(StatType.Mana)) * (1f + GetPercentModifier(StatType.Mana));

    [Header("Mana Regen")]
    public NetworkVariable<float> net_BaseManaRegen = new(writePerm: NetworkVariableWritePermission.Server);
    public float TotalManaRegen => (net_BaseManaRegen.Value + GetModifier(StatType.ManaRegen)) * (1f + GetPercentModifier(StatType.ManaRegen));

    [Header("Currency")]
    public float Coins;

    public enum PlayerClass
    {
        Beginner,
        Warrior,
        Magician,
        Archer,
        Rogue
    }

    public PlayerClass playerClass;

    public UnityEvent OnAPGained;
    public void ConsumeAttributePoints(int amount)
    {
        if (IsServer)
        {
            AttributePoints.Value -= amount;
        }
        else
        {
            ConsumeAttributePointsServerRPC(amount);
        }
    }

    [ServerRpc]
    void ConsumeAttributePointsServerRPC(int amount)
    {
        AttributePoints.Value -= amount;
    }

    public void IncreaseAttribuePoints()
    {
        if (IsServer)
        {
            AttributePoints.Value += ScalingData.AttributePointsPerLevel;
            OnAPGained?.Invoke();
        }
        else
        {
            IncreaseAttribuePointsServerRPC(ScalingData.AttributePointsPerLevel);
        }
    }

    [ServerRpc]
    void IncreaseAttribuePointsServerRPC(int amount)
    {
        AttributePoints.Value += amount;
        OnAPGained?.Invoke();
    }

    protected override void ApplyStatChange(StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.Mana: net_BaseMana.Value += amount; break;
            case StatType.ManaRegen: net_BaseManaRegen.Value += amount; break;
            case StatType.Endurance: net_BaseEndurance.Value += amount; break;
            case StatType.EnduranceRegen: net_BaseEnduranceRegen.Value += amount; break;
            default: base.ApplyStatChange(stat, amount); break;
        }
    }
}