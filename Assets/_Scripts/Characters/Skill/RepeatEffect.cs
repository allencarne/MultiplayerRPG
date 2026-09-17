using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Repeat Effect")]
public class RepeatEffect : SkillEffect
{
    [Header("Wrapper")]
    public SkillEffect InnerEffect;

    [Header("Repeat Settings")]
    [Min(1)]
    public int RepeatAmount = 1;
    [Min(0f)]
    public float RepeatInterval = 0f;

    public override int GetRepeatCount() => Mathf.Max(1, RepeatAmount);
    public override float GetRepeatInterval() => RepeatInterval;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        if (InnerEffect == null) return;
        // Delegate a single execution to the inner effect.
        // SkillEffectRelay will schedule this Execute() to run RepeatAmount times with RepeatInterval spacing.
        InnerEffect.Execute(owner, ctx);
    }
}
