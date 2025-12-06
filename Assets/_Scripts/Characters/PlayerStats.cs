using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Header("Customization")]
    public NetworkVariable<int> net_CharacterSlot = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString32Bytes> net_playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> net_bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Color> net_hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

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
}