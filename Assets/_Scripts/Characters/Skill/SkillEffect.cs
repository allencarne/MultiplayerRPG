using UnityEngine;

public abstract class SkillEffect : ScriptableObject
{
    // The number of times this effect should be executed. Default is 1.
    public virtual int GetRepeatCount() => 1;

    // The interval in seconds between each execution of this effect. Default is 0 (no delay).
    public virtual float GetRepeatInterval() => 0f;

    // Execute the effect on the given state machine with the provided skill context.
    public abstract void Execute(StateMachine owner, SkillContext ctx);

    // Find the DamageEffect if this SkillEffect is a DamageEffect, otherwise return null.
    public virtual DamageEffect FindDamageEffect() => this as DamageEffect;
}
