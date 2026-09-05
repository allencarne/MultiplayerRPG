using Unity.Netcode;
using UnityEngine;

public abstract class StateMachine : NetworkBehaviour
{
    public CharacterStats Stats;
    public CharacterAnimator Animator;
    public Collider2D Collider2D;
    public Rigidbody2D RigidBody2D;

    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public Mobility Mobility;

    public CastBar CastBar;

    private void Awake()
    {
        Stats = GetComponent<CharacterStats>();
        Animator = GetComponentInChildren<CharacterAnimator>();
        Collider2D = GetComponent<Collider2D>();
        RigidBody2D = GetComponent<Rigidbody2D>();

        CrowdControl = GetComponent<CrowdControl>();
        Buffs = GetComponent<Buffs>();
        DeBuffs = GetComponent<DeBuffs>();
        Mobility = GetComponent<Mobility>();

        CastBar = GetComponentInChildren<CastBar>();
    }

    public virtual Vector2 CurrentMoveInput => Vector2.zero;
    protected abstract ActiveSkillData GetSkillData(ActiveSkillData.SkillType type, int index);

    public void RequestSpawn(SkillContext context, NetworkedSpawnEffect effect)
    {
        if (IsServer)
        {
            context = ResolveServerContext(context);
            effect.SpawnServer(this, context);
        }
        else
        {
            RequestSpawnServerRpc(context);
        }
    }

    public void SpawnSingle(NetworkedSpawnEffect effect, SkillContext context)
    {
        if (!IsServer) return;

        GameObject instance = Instantiate(effect.Prefab, context.SpawnPosition + context.AimOffset, context.AimRotation);
        instance.transform.localScale *= transform.lossyScale.x;
        NetworkObject networkObject = instance.GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError($"SpawnEffect prefab '{effect.Prefab.name}' " + "does not have a NetworkObject.");
            Destroy(instance);
            return;
        }

        networkObject.Spawn();

        effect.Configure(instance, this, context);
    }

    [ServerRpc]
    void RequestSpawnServerRpc(SkillContext context)
    {
        context = ResolveServerContext(context);
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        SkillEffect effect = data.GetEffects(context.Phase)[context.EffectIndex];

        if (effect is not NetworkedSpawnEffect spawnEffect)
        {
            Debug.LogError($"Effect {context.EffectIndex} is not a NetworkedSpawnEffect.");

            return;
        }

        spawnEffect.SpawnServer(this, context);
    }

    public SkillContext ResolveServerContext(SkillContext context)
    {
        context.Attacker = NetworkObject;
        context.IsBasic = context.SkillType == ActiveSkillData.SkillType.Basic;
        context.AttackerDamage = Stats.TotalDamage;
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);

        context.AimRotation = Quaternion.Euler(0, 0, Mathf.Atan2(context.AimDirection.y, context.AimDirection.x) * Mathf.Rad2Deg);
        context.AimOffset = data.TargetingMode == ActiveSkillData.Targeting.Ground
            ? Vector2.zero
            : context.AimDirection.normalized * data.SkillRange;

        return context;
    }

    public float GetSkillRange(SkillContext context)
    {
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        return data != null ? data.SkillRange : 0f;
    }

    public bool IsGroundTargeted(SkillContext context)
    {
        ActiveSkillData data = GetSkillData(context.SkillType, context.SkillIndex);
        return data != null && data.TargetingMode == ActiveSkillData.Targeting.Ground;
    }
}
