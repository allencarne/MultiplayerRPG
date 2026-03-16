using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/New Skill")]
public class SkillData : ScriptableObject
{
    public enum SkillType { Basic, Offensive, Mobility, Defensive, Utility, Ultimate }
    public SkillType skillType;

    [Header("UI")]
    public Sprite SkillIcon;
    public GameObject IndicatorPrefab;
    [TextArea] public string Description;

    [Header("Prefabs")]
    public GameObject SkillPrefab;
    public GameObject TelegraphPrefab;

    [Header("Stats")]
    public float SkillDamage;
    public float SkillRange;

    [Header("Projectile")]
    public float SkillForce;
    public float SkillDuration;

    [Header("Time")]
    public float CastTime;
    public float ActionTime;
    public float ImpactTime;
    public float RecoveryTime;

    [Header("Cooldown")]
    public float CoolDown;

    [Header("Heal")]
    public int HealAmount;

    [Header("Slide")]
    public int SlideForce;
    public float SlideDuration;

    [Header("Slow")]
    public int SlowStacks;
    public float SlowDuration;

    [Header("Knockback")]
    public float KnockBackForce;
    public float KnockBackDuration;

    [Header("Stun")]
    public float StunDuration;
}
