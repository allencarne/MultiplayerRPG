
[System.Serializable]
public class StatModifier
{
    public float value;
    public StatType statType;
    public ModSource source;
    public ModType modType;
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

public enum ModType
{
    Flat,
    Percentage
}