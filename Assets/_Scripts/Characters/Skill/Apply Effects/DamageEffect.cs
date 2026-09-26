using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Damage Effect")]
public class DamageEffect : ApplyEffect
{
    [Header("Type of Damage")]
    public DamageType DamageType;

    [Header("Damage Formula")]
    [Tooltip("Flat base damage")]
    public float BaseDamage;
    [Tooltip("Ratio applied to attacker's TotalDamage (0 = none, 1 = full stat, like an AD/AP ratio)")]
    public float DamageRatio = 1f;
    [Tooltip("Flat bonus damage per attacker level")]
    public float PerLevelBonus;

    [Header("On successful hit, applies to the ATTACKER, not the target")]
    public SkillEffect[] OnDamageDealtEffects;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (!ctx.Attacker.TryGet(out NetworkObject attacker)) return;

        CharacterStats attackerStats = attacker.GetComponent<CharacterStats>();

        int attackerLevel = 1;
        PlayerStats ps = attacker.GetComponent<PlayerStats>();
        if (ps != null) attackerLevel = ps.PlayerLevel.Value;
        else
        {
            Enemy enemyComp = attacker.GetComponent<Enemy>();
            if (enemyComp != null && enemyComp.Data != null) attackerLevel = enemyComp.Data.Enemy_Level;
            else
            {
                NPC npcComp = attacker.GetComponent<NPC>();
                if (npcComp != null && npcComp.Data != null) attackerLevel = npcComp.Data.NPC_Level;
            }
        }

        float attackStatVal = attackerStats != null ? attackerStats.TotalDamage : 0f;
        float computedDamage = BaseDamage + (attackStatVal * DamageRatio) + (attackerLevel * PerLevelBonus) + ctx.AttackerDamage;

        float dealt = damageable.TakeDamage(computedDamage, DamageType, attacker, target.transform.position);

        if (ctx.IsBasic && attackerStats != null && attackerStats.TotalVamp > 0f)
        {
            float healAmount = dealt * (attackerStats.TotalVamp / 100f);
            if (attackerStats.net_TotalHP.Value != attackerStats.net_CurrentHP.Value)
                attackerStats.GiveHeal(healAmount, HealType.Flat);
        }

        if (OnDamageDealtEffects != null && OnDamageDealtEffects.Length > 0)
        {
            SkillContext selfCtx = ctx;
            selfCtx.Target = null;
            selfCtx.LastDamageDealt = dealt;
            foreach (SkillEffect effect in OnDamageDealtEffects) effect.Execute(owner, selfCtx);
        }
    }
}
