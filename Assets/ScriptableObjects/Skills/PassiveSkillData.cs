using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Data/Passive Skill")]
public class PassiveSkillData : SkillData
{
    [Header("Effects")]
    public SkillEffect[] OnActivateEffects;
}
