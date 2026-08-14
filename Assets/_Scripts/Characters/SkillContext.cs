using UnityEngine;

public struct SkillContext
{
    public Vector2 SpawnPosition;
    public Vector2 AimDirection;
    public Vector2 AimOffset;
    public Quaternion AimRotation;
    public float AttackerDamage;
    public bool IsBasic;
    public ulong AttackerId;
}
