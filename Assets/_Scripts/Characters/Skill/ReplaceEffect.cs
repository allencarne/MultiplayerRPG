using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Replace Effect")]
public class ReplaceEffect : SkillEffect
{
    [Header("Requirement")]
    public PassiveSkillData RequiredPassive;

    [Header("Effects")]
    public SkillEffect DefaultEffect;
    public SkillEffect AlternateEffect;

    // Note: GetRepeatCount/GetRepeatInterval are called by the relay BEFORE Execute()
    // and do not receive the owner context. Keep repeat settings compatible between default/alternate.
    public override int GetRepeatCount()
    {
        if (DefaultEffect != null) return DefaultEffect.GetRepeatCount();
        if (AlternateEffect != null) return AlternateEffect.GetRepeatCount();
        return 1;
    }

    public override float GetRepeatInterval()
    {
        if (DefaultEffect != null) return DefaultEffect.GetRepeatInterval();
        if (AlternateEffect != null) return AlternateEffect.GetRepeatInterval();
        return 0f;
    }

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        if (DefaultEffect == null && AlternateEffect == null) return;

        // if owner is a player, check passives; otherwise fall back to default.
        PlayerStateMachine player = owner as PlayerStateMachine;

        bool hasPassive = (RequiredPassive != null && player != null && player.HasPassive(RequiredPassive));

        SkillEffect toRun = hasPassive ? (AlternateEffect ?? DefaultEffect) : DefaultEffect;
        toRun?.Execute(owner, ctx);
    }
}
