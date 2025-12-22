
[System.Serializable]
public class StatModifier
{
    public float value;
    public StatType statType;
    public ModSource source;
}

public enum StatType
{
    Damage,
    Health,
    AttackSpeed,
    CoolDown,
    Speed,
    Armor
}

public enum ModSource
{
    Equipment,
    Buff,
    Debuff
}