using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    public int EffectID;

    public abstract void Execute(PlayerStateMachine owner, SkillContext ctx);
}
