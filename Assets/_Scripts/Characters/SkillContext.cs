using Unity.Netcode;
using UnityEngine;

public struct SkillContext : INetworkSerializable
{
    public Vector2 SpawnPosition;
    public Vector2 AimDirection;
    public SkillData.SkillType SkillType;
    public int SkillIndex;
    public PlayerSkill.State Phase;
    public int EffectIndex;

    // server-only working fields — never serialized
    public Vector2 AimOffset;
    public Quaternion AimRotation;
    public float AttackerDamage;
    public bool IsBasic;
    public ulong AttackerId;
    public NetworkObject Target;
    public float LastDamageDealt;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SpawnPosition);
        serializer.SerializeValue(ref AimDirection);
        serializer.SerializeValue(ref SkillType);
        serializer.SerializeValue(ref SkillIndex);
        serializer.SerializeValue(ref Phase);
        serializer.SerializeValue(ref EffectIndex);
    }
}
