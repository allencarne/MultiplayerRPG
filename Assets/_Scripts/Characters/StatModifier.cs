
using System;
using Unity.Netcode;

[System.Serializable]
public struct StatModifier : INetworkSerializable, IEquatable<StatModifier>
{
    public float value;
    public StatType statType;
    public ModSource source;
    public ModType modType;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref value);
        serializer.SerializeValue(ref statType);
        serializer.SerializeValue(ref source);
        serializer.SerializeValue(ref modType);
    }

    public bool Equals(StatModifier other)
    {
        return value.Equals(other.value) && statType == other.statType && source == other.source && modType == other.modType;
    }
}

public enum StatType
{
    Damage,
    Health,
    AttackSpeed,
    CoolDown,
    Speed,
    Armor,
    Vamp
}

public enum ModSource
{
    Equipment,
    Buff,
    Debuff
}

public enum ModType
{
    Flat,
    Percent
}