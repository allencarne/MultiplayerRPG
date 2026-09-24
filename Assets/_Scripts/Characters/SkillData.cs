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
}
