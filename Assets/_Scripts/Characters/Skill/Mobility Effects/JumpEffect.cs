using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Mobility/Jump Effect")]
public class JumpEffect : SkillEffect
{
    [Header("Leap")]
    public float Duration = 0.5f;
    public float Height = 1f;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        owner.Mobility.Jump(ctx.SpawnPosition,Duration,Height);
    }
}
