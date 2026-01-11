using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : CharacterStats
{
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
    public NetworkVariable<float> Endurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxEndurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> EnduranceRechargeRate = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Fury")]
    public NetworkVariable<float> Fury = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxFury = new(writePerm: NetworkVariableWritePermission.Server);

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

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TakeDamage(1,DamageType.Flat, NetworkObject);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GiveHeal(1, HealType.Flat);
        }
    }

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
        if (PlayerLevel.Value < 10)
        {
            if (IsServer)
            {
                AttributePoints.Value += 1;
            }
            else
            {
                IncreaseAttribuePointsServerRPC(1);
            }
        }
        else
        {
            if (IsServer)
            {
                AttributePoints.Value += 3;
            }
            else
            {
                IncreaseAttribuePointsServerRPC(3);
            }
        }

        OnAPGained?.Invoke();
    }

    [ServerRpc]
    void IncreaseAttribuePointsServerRPC(int amount)
    {
        AttributePoints.Value += amount;
    }
}