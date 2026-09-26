using UnityEngine;

public abstract class SkillData : ScriptableObject
{
    [Header("UI")]
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;

    [Header("Cooldown")]
    public float CoolDown;

    [Header("Cost")]
    public float ManaCost = 0f;

    // Find the damage effect of this skill, if any. This is used for tooltips and other UI elements.
    public virtual DamageEffect FindDamageEffect() => null;
}
