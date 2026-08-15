using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Damage Effect")]
public class DamageEffect : ApplyEffect
{
    public float Damage;
    public DamageType DamageType;

    [Header("On successful hit, apply these to the attacker")]
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

        if (OnDamageDealtEffects != null && OnDamageDealtEffects.Length > 0)
        {
            SkillContext selfCtx = ctx;
            selfCtx.Target = null;
            selfCtx.LastDamageDealt = dealt;
            foreach (SkillEffect effect in OnDamageDealtEffects) effect.Execute(owner, selfCtx);
        }
    }
}
