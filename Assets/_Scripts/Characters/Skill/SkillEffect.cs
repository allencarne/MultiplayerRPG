using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int EffectID;

    public virtual int GetRepeatCount() => 1;
    public virtual float GetRepeatInterval() => 0f;

    public abstract void Execute(PlayerStateMachine owner, SkillContext ctx);
}
