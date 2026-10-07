using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/UI/Combat Text Settings")]
public class CombatTextSettings : ScriptableObject
{
    [Header("Damage skins (index = skin)")]
    public GameObject[] Hurt;
    public GameObject[] Crit;
    public GameObject[] Bleed;

    [Header("Self text")]
    public GameObject Heal, Exp, Level, Buff, Debuff;

    public float spawnRadius = 1.2f;

    public GameObject GetHit(HitType type, int skin)
    {
        GameObject[] set = type switch
        {
            HitType.Crit => Crit,
            HitType.Bleed => Bleed,
            _ => Hurt
        };
        if (set == null || set.Length == 0) return null;
        return set[Mathf.Clamp(skin, 0, set.Length - 1)]; // bad index falls back safely
    }
}
