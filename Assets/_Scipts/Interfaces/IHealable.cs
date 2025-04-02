
public interface IHealable
{
    void GiveHeal(float healAmount, HealType healType);
}

public enum HealType
{
    Flat,       // Example: +10 HP
    Percentage  // Example: +10% of max HP
}
