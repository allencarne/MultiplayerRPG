using Unity.Netcode;
using UnityEngine;

public interface IDamageable
{
    float TakeDamage(float damage, DamageType damageType, NetworkObject attackerID, Vector2 position);
}

public enum DamageType
{
    Flat,                       // e.g. -10 HP, reduced by armor
    True,                       // ignores armor entirely

    PercentMaxHealth,           // % of target's max HP, reduced by armor
    PercentMaxHealthTrue,       // % of target's max HP, ignores armor

    PercentMissingHealth,       // % of target's missing HP, reduced by armor
    PercentMissingHealthTrue,   // % of target's missing HP, ignores armor (execute-style, like Warwick's ult)

    PercentCurrentHealth,       // % of target's current HP, reduced by armor
    PercentCurrentHealthTrue    // % of target's current HP, ignores armor (like Vayne's Silver Bolts)
}