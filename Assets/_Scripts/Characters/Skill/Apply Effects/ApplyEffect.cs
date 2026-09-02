using Unity.Netcode;
using UnityEngine;

public abstract class ApplyEffect : SkillEffect
{
    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        NetworkObject target = ctx.Target != null ? ctx.Target : owner.NetworkObject;
        ApplyTo(target, owner, ctx);
    }

    protected abstract void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx);
}
