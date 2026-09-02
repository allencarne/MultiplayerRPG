using UnityEngine;

public abstract class NetworkedSpawnEffect : SkillEffect
{
    [Header("Prefab")]
    public GameObject Prefab;

    [Header("Spawn Position")]
    public bool UseCurrentPosition;
    public sealed override void Execute(StateMachine owner, SkillContext ctx)
    {
        if (UseCurrentPosition)
        {
            ctx.SpawnPosition = owner.transform.position;
        }
        owner.RequestSpawn(ctx, this);
    }

    public virtual void SpawnServer(StateMachine owner, SkillContext ctx)
    {
        owner.SpawnSingle(this, ctx);
    }

    public abstract void Configure(GameObject instance, StateMachine owner, SkillContext ctx);
}
