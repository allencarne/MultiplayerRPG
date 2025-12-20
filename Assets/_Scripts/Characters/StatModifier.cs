
[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public int value;
    public ModSource source;
}

public enum StatType
{
    Damage,
    Health,
    AttackSpeed,
    CoolDown,
    Speed
}

public enum ModSource
{
    Equipment,
    Buff,
    Debuff
}

public enum ModifierType
{
    Flat,
    Percentage
}