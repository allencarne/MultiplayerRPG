
using Unity.Netcode;

public interface IDamageable
{
    void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID);
}

public enum DamageType
{
    Flat,       // Example: -10 HP
    Percent,    // Example: -10% of max HP
    True        // Ignores Armor
}