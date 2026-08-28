using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Damage Effect")]
public class DamageEffect : ApplyEffect
{
    [Header("Amount of Damage")]
    public float Damage;

    [Header("Type of Damage")]
    public DamageType DamageType;

    [Header("On successful hit, applies to the ATTACKER, not the target")]
    public SkillEffect[] OnDamageDealtEffects;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        // Get Damageable Component
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable == null) return;

        // Find the Attacker's NetworkObject
        NetworkObject attacker = NetworkManager.Singleton.ConnectedClients[ctx.AttackerId].PlayerObject;

        // Apply Damage to the Attacker
        float dealt = damageable.TakeDamage(ctx.AttackerDamage + Damage, DamageType, attacker, target.transform.position);

        // Apply Vamp (Life Steal)
        if (ctx.IsBasic)
        {
            CharacterStats attackerStats = attacker.GetComponent<CharacterStats>();
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
