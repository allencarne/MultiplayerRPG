using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int EffectID;

    public abstract void Execute(PlayerStateMachine owner, SkillContext ctx);
}
