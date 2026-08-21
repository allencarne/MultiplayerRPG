using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int EffectID;

    // How long this effect needs to fully play out. Default: instant/no delay.
    public virtual float GetEffectDuration() => 0f;

    public abstract void Execute(PlayerStateMachine owner, SkillContext ctx);
}
