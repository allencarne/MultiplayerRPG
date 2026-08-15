using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "Scriptable Objects/Skill Effects/Apply Effect")]
public class ApplyEffect : SkillEffect
{
    public enum EffectKind { CrowdControl, Buff, Debuff, Heal }
    public EffectKind Effect;
    public int Stacks = 1;
    public float Duration;

    public override void Execute(PlayerStateMachine owner, SkillContext ctx)
    {
        Debug.Log("Execute Apply Effect");
        // owner.ApplyEffect(this, ctx) — dispatch by Effect kind, apply to self
    }
}
