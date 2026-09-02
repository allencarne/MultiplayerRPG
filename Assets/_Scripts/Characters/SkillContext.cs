using Unity.Netcode;
using UnityEngine;

public struct SkillContext : INetworkSerializable
{
    public Vector2 SpawnPosition;
    public Vector2 AimDirection;
    public ActiveSkillData.SkillType SkillType;
    public int SkillIndex;
    public ActiveSkillData.SkillPhase Phase;
    public int EffectIndex;

    // server-only working fields — never serialized
    public Vector2 AimOffset;
    public Quaternion AimRotation;
    public float AttackerDamage;
    public bool IsBasic;
    public NetworkObjectReference Attacker;
    public NetworkObject Target;
    public float LastDamageDealt;

    // Telegraph
    public float FillDuration;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SpawnPosition);
        serializer.SerializeValue(ref AimDirection);
        serializer.SerializeValue(ref SkillType);
        serializer.SerializeValue(ref SkillIndex);
        serializer.SerializeValue(ref Phase);
        serializer.SerializeValue(ref EffectIndex);
        serializer.SerializeValue(ref FillDuration);
    }
}
