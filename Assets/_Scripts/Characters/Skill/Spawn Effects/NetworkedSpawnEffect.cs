using UnityEngine;

public abstract class NetworkedSpawnEffect : SkillEffect
{
    [Header("Prefab")]
    public GameObject Prefab;

    [Header("Spawn Position")]
    public bool UseCurrentPosition;

    [Header("Cancel on Crowd Control")]
    public bool CancelOnInterrupt = false;
    public bool CancelOnStagger = false;

    public sealed override void Execute(StateMachine owner, SkillContext ctx)
    {
        if (UseCurrentPosition)
        {
            ctx.SpawnPosition = owner.transform.position;
        }

        if (owner != null && owner.Pathfinding != null)
        {
            LayerMask mask = owner.Pathfinding.obstacleLayerMask;
            ctx.SpawnPosition = owner.Pathfinding.GetValidGroundPosition(ctx.SpawnPosition, mask);
        }

        owner.RequestSpawn(ctx, this);
    }

    public virtual void SpawnServer(StateMachine owner, SkillContext ctx)
    {
        owner.SpawnSingle(this, ctx);
    }

    public abstract void Configure(GameObject instance, StateMachine owner, SkillContext ctx);
}
