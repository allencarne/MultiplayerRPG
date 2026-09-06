using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Mobility/Blink Effect")]
public class BlinkEffect : SkillEffect
{
    [Header("Blink")]
    public float Distance = 5f;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        owner.Mobility.Blink(ctx.AimDirection,Distance);
    }
}
