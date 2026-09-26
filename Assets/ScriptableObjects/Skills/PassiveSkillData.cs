using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Data/Passive Skill")]
public class PassiveSkillData : SkillData
{
    [Header("Trigger")]
    public PassiveTrigger Trigger;

    [Header("Effects")]
    public SkillEffect[] OnActivateEffects;

    public override DamageEffect FindDamageEffect()
    {
        if (OnActivateEffects == null) return null;

        foreach (SkillEffect effect in OnActivateEffects)
        {
            if (effect == null) continue;
            DamageEffect found = effect.FindDamageEffect();
            if (found != null) return found;
        }
        return null;
    }
}
