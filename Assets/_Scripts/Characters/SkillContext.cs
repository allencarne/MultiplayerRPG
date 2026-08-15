using Unity.Netcode;
using UnityEngine;

public struct SkillContext : INetworkSerializable
{
    public Vector2 SpawnPosition;
    public Vector2 AimDirection;
    public Vector2 AimOffset;
    public Quaternion AimRotation;
    public float AttackerDamage;
    public bool IsBasic;
    public ulong AttackerId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SpawnPosition);
        serializer.SerializeValue(ref AimDirection);
        serializer.SerializeValue(ref AimOffset);
        serializer.SerializeValue(ref AimRotation);
        serializer.SerializeValue(ref AttackerDamage);
        serializer.SerializeValue(ref IsBasic);
        serializer.SerializeValue(ref AttackerId);
    }
}
