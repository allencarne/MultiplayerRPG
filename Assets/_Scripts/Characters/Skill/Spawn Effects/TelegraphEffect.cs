using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Networked Spawn/Telegraph Effect")]
public class TelegraphEffect : NetworkedSpawnEffect
{
    public override void Configure(GameObject instance, PlayerStateMachine owner, SkillContext ctx)
    {
        ITelegraph telegraph = instance.GetComponent<ITelegraph>();
        telegraph?.Init(owner.Stats, ctx.FillDuration);
    }
}
