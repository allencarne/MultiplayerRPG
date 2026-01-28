using Unity.Netcode;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID, Vector2 position);
}

public enum DamageType
{
    Flat,       // Example: -10 HP
    Percent,    // Example: -10% of max HP
    True        // Ignores Armor
}