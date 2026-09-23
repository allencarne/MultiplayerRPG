using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Conditional/Requires Passive")]
public class RequiresPassiveEffect : SkillEffect
{
    [Header("Requirement")]
    public PassiveSkillData RequiredPassive;

    [Header("Wrapped Effect")]
    public SkillEffect InnerEffect;

    public override int GetRepeatCount() => InnerEffect != null ? InnerEffect.GetRepeatCount() : 1;
    public override float GetRepeatInterval() => InnerEffect != null ? InnerEffect.GetRepeatInterval() : 0f;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        if (RequiredPassive == null || InnerEffect == null) return;

        // Only players can have passives; if owner is not a PlayerStateMachine then skip.
        PlayerStateMachine player = owner as PlayerStateMachine;
        if (player == null) return;

        if (!player.HasPassive(RequiredPassive)) return;

        InnerEffect.Execute(owner, ctx);
    }
}
