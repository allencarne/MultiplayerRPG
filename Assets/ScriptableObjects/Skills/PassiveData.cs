using UnityEngine;

public class PassiveData : ScriptableObject
{
    [Header("UI")]
    public string PassiveName;
    public Sprite PassiveIcon;
    [TextArea] public string Description;

    [Header("Cooldown")]
    public float CoolDown;
}
