
public interface IDamageable
{
    void TakeDamage(float damage, DamageType damageType);
}

public enum DamageType
{
    Flat,       // Example: -10 HP
    Percentage  // Example: -10% of max HP
}