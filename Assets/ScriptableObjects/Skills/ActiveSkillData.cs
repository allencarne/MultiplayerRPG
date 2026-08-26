using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Data/Active Skill")]
public class ActiveSkillData : SkillData
{
    public WeaponType weaponType;

    public enum SkillType { Basic, Offensive, Mobility, Defensive, Utility, Ultimate }
    public SkillType skillType;

    public enum Targeting { Directional, Ground }
    public Targeting TargetingMode = Targeting.Directional;

    public enum ImpactAnimationStyle { Normal, Long, Repeated }

    [Header("Impact Style")]
    public ImpactAnimationStyle ImpactStyle = ImpactAnimationStyle.Normal;

    [Header("Prefabs")]
    public GameObject SkillPrefab;
    public GameObject IndicatorPrefab;
    public GameObject TelegraphPrefab;

    [Header("Stats")]
    public float SkillRange;

    [Header("Time")]
    public float CastTime;
    public float ActionTime;
    public float ImpactTime;
    public float RecoveryTime;

    [Header("Effects")]
    public SkillEffect[] OnCastEffects;
    public SkillEffect[] OnActionEffects;
    public SkillEffect[] OnImpactEffects;
    public SkillEffect[] OnRecoveryEffects;

    public SkillEffect[] GetEffects(PlayerSkill.State phase) => phase switch
    {
        PlayerSkill.State.Cast => OnCastEffects,
        PlayerSkill.State.Action => OnActionEffects,
        PlayerSkill.State.Impact => OnImpactEffects,
        PlayerSkill.State.Recovery => OnRecoveryEffects,
        _ => null
    };
}
