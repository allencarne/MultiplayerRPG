using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Damage Effect")]
public class DamageEffect : ApplyEffect
{
    [Header("Amount of Damage")]
    public float Damage;

    [Header("Type of Damage")]
    public DamageType DamageType;

    [Header("Formula override")]
    [Tooltip("When enabled, damage will be calculated by the formula below instead of using the 'Damage' field.")]
    public bool UseFormula = false;

    [Header("Formula multipliers")]
    [Tooltip("Multiplier applied to the attacker's damage stat")]
    public float StatMultiplier = 1f;
    [Tooltip("Multiplier applied to the attacker's level")]
    public float LevelMultiplier = 0.5f;
    [Tooltip("Optional additional flat base added to the formula (can be 0)")]
    public float AdditionalBase = 0f;

    [Header("Skill type multipliers")]
    public float BasicMultiplier = 1f;
    public float OffensiveMultiplier = 1.25f;
    public float MobilityMultiplier = 1f;
    public float DefensiveMultiplier = 0.8f;
    public float UtilityMultiplier = 1f;
    public float UltimateMultiplier = 2f;

    [Header("On successful hit, applies to the ATTACKER, not the target")]
    public SkillEffect[] OnDamageDealtEffects;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        // Get Damageable Component (target)
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;

        // Find the Attacker's NetworkObject
        if (!ctx.Attacker.TryGet(out NetworkObject attacker)) return;

        // Gather attacker stats / level early (used for vamp and/or formula)
        CharacterStats attackerStats = attacker.GetComponent<CharacterStats>();

        // Resolve attacker level (player, enemy, NPC) - default to 1 if missing
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

        // Compute damage
        float computedDamage = Damage;

        if (UseFormula)
        {
            // Get attacker's damage stat value
            float attackStatVal = 0f;
            if (attackerStats != null) attackStatVal = attackerStats.TotalDamage;

            // Pick skill type multiplier
            float skillTypeMultiplier = ctx.SkillType switch
            {
                ActiveSkillData.SkillType.Basic => BasicMultiplier,
                ActiveSkillData.SkillType.Offensive => OffensiveMultiplier,
                ActiveSkillData.SkillType.Mobility => MobilityMultiplier,
                ActiveSkillData.SkillType.Defensive => DefensiveMultiplier,
                ActiveSkillData.SkillType.Utility => UtilityMultiplier,
                ActiveSkillData.SkillType.Ultimate => UltimateMultiplier,
                _ => 1f
            };

            // Formula: ((TotalDamage * StatMultiplier) + (Level * LevelMultiplier) + AdditionalBase) * SkillMultiplier
            float formulaDamage = ((attackStatVal * StatMultiplier) + (attackerLevel * LevelMultiplier) + AdditionalBase) * skillTypeMultiplier;

            // Preserve ctx.AttackerDamage as an additive bonus for formula-driven skills
            computedDamage = formulaDamage + ctx.AttackerDamage;
        }
        else
        {
            // When not using the formula, apply exactly the SO value and DO NOT add ctx.AttackerDamage.
            computedDamage = Damage;
        }

        // Apply Damage to the Target
        float dealt = damageable.TakeDamage(computedDamage, DamageType, attacker, target.transform.position);

        // Apply Vamp (Life Steal) on basic hits (unchanged behavior)
        if (ctx.IsBasic)
        {
            if (attackerStats != null && attackerStats.TotalVamp > 0f)
            {
                float healAmount = dealt * (attackerStats.TotalVamp / 100f);
                attackerStats.GiveHeal(healAmount, HealType.Flat);
            }
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
