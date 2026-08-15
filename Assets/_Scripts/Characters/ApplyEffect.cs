using Unity.Netcode;
using UnityEngine;

public abstract class ApplyEffect : SkillEffect
{
    public override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        NetworkObject target = ctx.Target != null ? ctx.Target : owner.NetworkObject;
        ApplyTo(target, owner, ctx);
    }

    protected abstract void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx);
}
