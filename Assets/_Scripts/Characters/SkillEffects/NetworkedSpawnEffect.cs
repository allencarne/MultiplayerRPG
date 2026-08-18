using UnityEngine;

public abstract class NetworkedSpawnEffect : SkillEffect
{
    public GameObject Prefab;
    public bool UseCurrentPosition;
    public sealed override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        if (UseCurrentPosition)
        {
            ctx.SpawnPosition = owner.transform.position;
        }
        owner.RequestSpawn(ctx, this);
    }

    public abstract void Configure(GameObject instance, PlayerStateMachine owner, SkillContext ctx);
}
