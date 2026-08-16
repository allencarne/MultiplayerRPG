using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Scriptable Objects/Skill Effects/Spawn Effect")]
public class SpawnEffect: SkillEffect
{
    public GameObject Prefab;
    public int Amount = 1;
    public float Force;
    public float Duration;
    public float SpreadAngle;
    public int RepeatAmount = 1;
    public float RepeatRate;

    public bool IgnorePlayer;
    public bool IgnoreEnemy;
    public bool IgnoreNPC;

    public bool UseCurrentPosition;

    [Header("What this spawned thing does when it hits something")]
    public SkillEffect[] OnTriggerEffects;

    public override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        if (UseCurrentPosition)
        {
            ctx.SpawnPosition = owner.transform.position;
            ctx.AimOffset = ctx.AimDirection.normalized * ctx.AimOffset.magnitude;
        }

        // Temporary
        Debug.Log("Execute Spawn Effect: " + ctx.Phase);
        owner.RequestAttack(ctx, this);

        //if (RepeatAmount <= 1) SpawnBurst(owner, ctx);
        //else owner.StartCoroutine(RepeatSpawn(owner, ctx));
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
