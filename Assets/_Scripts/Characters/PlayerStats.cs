using Unity.Netcode;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Header("Endurance")]
    public NetworkVariable<float> Endurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> EnduranceRechargeRate = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxEndurance = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Fury")]
    public NetworkVariable<float> Fury = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxFury = new(writePerm: NetworkVariableWritePermission.Server);
}
