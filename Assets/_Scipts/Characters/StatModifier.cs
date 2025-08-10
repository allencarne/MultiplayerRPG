
[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public int value;
}

public enum StatType
{
    Damage,
    Health,
    AttackSpeed,
    CoolDown
}
