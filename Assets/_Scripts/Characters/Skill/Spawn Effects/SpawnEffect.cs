using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Networked Spawn/Spawn Effect")]
public class SpawnEffect: NetworkedSpawnEffect
{
    public enum SpreadMode
    {
        None,
        EvenCone,
        RandomCone,
        Radial
    }

    [Header("Spawn")]
    [Min(1)]
    public int Amount = 1;

    [Min(1)]
    public int RepeatAmount = 1;

    [Min(0f)]
    public float RepeatRate = 0f;

    [Header("Spread")]
    public SpreadMode spreadMode = SpreadMode.None;

    [Min(0f)]
    [Range(0f, 360f)]
    public float SpreadAngle = 0f;

    [Header("Movement")]
    public float Force = 0f;
    public ForceMode2D ForceMode = ForceMode2D.Impulse;

    [Tooltip("Rotates each spawned prefab to match its launch direction.")]
    public bool RotateToDirection = true;

    [Header("Lifetime")]
    [Min(0f)]
    public float Duration = 0f;

    [Header("Target")]
    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    [Header("Collision")]
    public bool IsBreakable;

    [Header("What this spawned thing does when it hits something")]
    public SkillEffect[] OnTriggerEffects;

    public override void SpawnServer(StateMachine owner, SkillContext ctx)
    {
        owner.StartCoroutine(SpawnRoutine(owner, ctx));
    }

    IEnumerator SpawnRoutine(StateMachine owner, SkillContext ctx)
    {
        int repeatCount = Mathf.Max(1, RepeatAmount);

        for (int repeat = 0; repeat < repeatCount; ++repeat)
        {
            SpawnBurst(owner, ctx);

            if (repeat < repeatCount - 1 && RepeatRate > 0f)
            {
                yield return new WaitForSeconds(RepeatRate);
            }
        }
    }

    void SpawnBurst(StateMachine owner, SkillContext ctx)
    {
        int amount = Mathf.Max(1, Amount);
        bool groundTargeted = owner.IsGroundTargeted(ctx);

        for (int i = 0; i < amount; i++)
        {
            Vector2 direction = ComputeDirection(ctx.AimDirection, i, amount);

            SkillContext spawnContext = ctx;
            spawnContext.AimDirection = direction;
            spawnContext.AimRotation = DirectionToRotation(direction);
            spawnContext.AimOffset = groundTargeted ? Vector2.zero : direction.normalized * owner.GetSkillRange(ctx);

            owner.SpawnSingle(this, spawnContext);
        }
    }

    Vector2 ComputeDirection(Vector2 baseDirection,int index,int amount)
    {
        baseDirection.Normalize();

        switch (spreadMode)
        {
            case SpreadMode.None: return baseDirection;

            case SpreadMode.EvenCone:
                {
                    if (amount <= 1)
                        return baseDirection;

                    float halfAngle = SpreadAngle * 0.5f;

                    float angle = Mathf.Lerp(
                        -halfAngle,
                        halfAngle,
                        index / (float)(amount - 1));

                    return RotateVector(baseDirection, angle);
                }

            case SpreadMode.RandomCone:
                {
                    float halfAngle = SpreadAngle * 0.5f;
                    float angle = Random.Range(-halfAngle, halfAngle);

                    return RotateVector(baseDirection, angle);
                }

            case SpreadMode.Radial:
                {
                    float angle = (360f / amount) * index;
                    return RotateVector(baseDirection, angle);
                }
        }

        return baseDirection;
    }

    Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;

        float x = vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians);
        float y = vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians);

        return new Vector2(x, y).normalized;
    }

    Quaternion DirectionToRotation(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, angle);
    }

    public override void Configure(GameObject instance, StateMachine owner, SkillContext ctx)
    {
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        if (rb != null && Force != 0f) rb.AddForce(ctx.AimDirection.normalized * Force, ForceMode);

        SkillEffectRelay relay = instance.GetComponent<SkillEffectRelay>();
        if (relay != null) relay.Initialize(owner, ctx, OnTriggerEffects, IgnorePlayer, IgnoreEnemy, IgnoreNPC, IsBreakable);

        //FollowTarget target = instance.GetComponent<FollowTarget>();
        //if (target != null) target.Target = owner.transform;

        DespawnDelay despawn = instance.GetComponent<DespawnDelay>();
        if (despawn != null && Duration > 0f) despawn.StartCoroutine(despawn.DespawnAfterDuration(Duration));

        DestroyOnDeath death = instance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = owner.Stats;
    }

    public override int GetRepeatCount() => Mathf.Max(1, RepeatAmount);
    public override float GetRepeatInterval() => RepeatRate;
}
