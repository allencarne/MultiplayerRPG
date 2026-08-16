using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Slide Effect")]
public class SlideEffect : SkillEffect
{
    public float Force;
    public float Duration;
    public bool RequireMoveInput;

    public override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        Vector2 direction = ctx.AimDirection;

        if (RequireMoveInput)
        {
            if (owner.Input.MoveInput == Vector2.zero) return; // rooted — don't slide
            direction = owner.Input.MoveInput;
        }

        owner.Mobility.Slide(direction, Force, Duration);
    }
}
