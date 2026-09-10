using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Mobility/Jump Effect")]
public class JumpEffect : SkillEffect
{
    [Header("Leap")]
    public float Duration = 0.5f;
    public float Height = 1f;

    public override void Execute(StateMachine owner, SkillContext ctx)
    {
        Vector2 targetPos = ctx.SpawnPosition;

        if (owner != null && owner.Pathfinding != null)
        {
            LayerMask mask = owner.Pathfinding.obstacleLayerMask;
            targetPos = owner.Pathfinding.GetValidGroundPosition(ctx.SpawnPosition, mask);
        }

        owner.Mobility.Jump(targetPos, Duration, Height);
    }
}
