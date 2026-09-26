using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Data/Active Skill")]
public class ActiveSkillData : SkillData
{
    public enum SkillType { Basic, Offensive, Mobility, Defensive, Utility, Ultimate }
    public enum Targeting { Directional, Ground }
    public enum SkillPhase { Cast, Action, Impact, Recovery, Done }
    public enum ImpactAnimationStyle { Normal, Long, Repeated }
    public enum AttackAnimation { WeaponBasicFront, WeaponBasicBack, Empower, Dash }

    [Header("Range")]
    public float SkillRange;

    [Header("Weapon Type")]
    public WeaponType weaponType;

    [Header("Skill Type")]
    public SkillType skillType;

    [Header("Animation")]
    public AttackAnimation Animation = AttackAnimation.WeaponBasicFront;

    [Header("Targeting Mode")]
    public Targeting TargetingMode = Targeting.Directional;

    [Header("Impact Style")]
    public ImpactAnimationStyle ImpactStyle = ImpactAnimationStyle.Normal;

    [Header("Indicator")]
    public GameObject IndicatorPrefab;

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

    public SkillEffect[] GetEffects(SkillPhase phase) => phase switch
    {
        SkillPhase.Cast => OnCastEffects,
        SkillPhase.Action => OnActionEffects,
        SkillPhase.Impact => OnImpactEffects,
        SkillPhase.Recovery => OnRecoveryEffects,
        _ => null
    };

    public override DamageEffect FindDamageEffect()
    {
        return FindInEffects(OnCastEffects)
            ?? FindInEffects(OnActionEffects)
            ?? FindInEffects(OnImpactEffects)
            ?? FindInEffects(OnRecoveryEffects);
    }

    DamageEffect FindInEffects(SkillEffect[] effects)
    {
        if (effects == null) return null;

        foreach (SkillEffect effect in effects)
        {
            if (effect == null) continue;
            DamageEffect found = effect.FindDamageEffect();
            if (found != null) return found;
        }
        return null;
    }
}
