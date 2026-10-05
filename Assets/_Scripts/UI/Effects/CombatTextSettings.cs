using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/UI/Combat Text Settings")]
public class CombatTextSettings : ScriptableObject
{
    public GameObject Hurt, Deal, Crit, Bleed, Heal, Exp, Level, Buff, Debuff;
    public float spawnRadius = 1.2f;

    public GameObject Get(HitType type) => type switch
    {
        HitType.Crit => Crit,
        HitType.Bleed => Bleed,
        _ => Hurt
    };
}
