using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Data/Passive Skill")]
public class PassiveSkillData : SkillData
{
    [Header("Trigger")]
    public PassiveTrigger Trigger;

    [Header("Effects")]
    public SkillEffect[] OnActivateEffects;
}
