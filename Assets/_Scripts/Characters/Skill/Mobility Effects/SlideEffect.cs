using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Slide Effect")]
public class SlideEffect : SkillEffect
{
    [Header("Slide")]
    public float Force;
    public float Duration;

    [Header("Movement")]
    public bool RequireMoveInput;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        Vector2 direction = ctx.AimDirection;
        if (RequireMoveInput) if (owner.CurrentMoveInput == Vector2.zero) return;
        owner.Mobility.Slide(direction, Force, Duration);
    }
}
