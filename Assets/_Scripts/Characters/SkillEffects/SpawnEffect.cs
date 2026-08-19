using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Scriptable Objects/Skill Effects/Networked Spawn/Spawn Effect")]
public class SpawnEffect: NetworkedSpawnEffect
{
    [Header("Data")]
    public int Amount = 1;
    public float Force;
    public float Duration;
    public float SpreadAngle;
    public int RepeatAmount = 1;
    public float RepeatRate;

    [Header("Target")]
    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    [Header("Breakable")]
    public bool IsBreakable;

    [Header("What this spawned thing does when it hits something")]
    public SkillEffect[] OnTriggerEffects;

    public override void Configure(GameObject instance, PlayerStateMachine owner, SkillContext ctx)
    {
        Rigidbody2D attackRB = instance.GetComponent<Rigidbody2D>();
        if (attackRB != null) attackRB.AddForce(ctx.AimDirection * Force, ForceMode2D.Impulse);

        SkillEffectRelay relay = instance.GetComponent<SkillEffectRelay>();
        if (relay != null) relay.Initialize(owner, ctx, OnTriggerEffects, IgnorePlayer, IgnoreEnemy, IgnoreNPC, IsBreakable);

        FollowTarget target = instance.GetComponent<FollowTarget>();
        if (target != null) target.Target = owner.transform;

        DestroyOnDeath death = instance.GetComponent<DestroyOnDeath>();
        if (death != null) death.stats = owner.Stats;

        DespawnDelay despawnDelay = instance.GetComponent<DespawnDelay>();
        if (despawnDelay != null) despawnDelay.StartCoroutine(despawnDelay.DespawnAfterDuration(Duration));
    }

    IEnumerator RepeatSpawn(PlayerStateMachine owner, SkillContext ctx)
    {
        for (int i = 0; i < RepeatAmount; i++)
        {
            SpawnBurst(owner, ctx);
            yield return new WaitForSeconds(RepeatRate);
        }
    }

    void SpawnBurst(PlayerStateMachine owner, SkillContext ctx)
    {
        for (int i = 0; i < Amount; i++)
        {
            //Vector2 dir = ComputeSpreadDirection(ctx.AimDirection, SpreadAngle, i, Amount);
            //owner.RequestSpawnEffect(this, dir, ctx); // networked spawn, lives on PlayerStateMachine
        }
    }
}
