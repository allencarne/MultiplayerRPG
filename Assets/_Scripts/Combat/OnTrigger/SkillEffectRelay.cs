using Unity.Netcode;
using UnityEngine;

public class SkillEffectRelay : NetworkBehaviour
{
    SkillEffect[] onTriggerEffects;
    SkillContext context;
    PlayerStateMachine owner;
    bool ignorePlayer, ignoreEnemy, ignoreNPC;

    public void Initialize(PlayerStateMachine _owner, SkillContext _ctx, SkillEffect[] _triggerEffects, bool _ignorePlayer, bool _ignoreEnemy, bool _ignoreNPC)
    {
        owner = _owner;
        context = _ctx;
        onTriggerEffects = _triggerEffects;
        ignorePlayer = _ignorePlayer;
        ignoreEnemy = _ignoreEnemy;
        ignoreNPC = _ignoreNPC;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only the server can apply effects
        if (!IsServer) return;

        // Return if there are no effects
        if (onTriggerEffects == null || onTriggerEffects.Length == 0) return;

        // Return if invalid target
        if (collision.CompareTag("Player") && ignorePlayer) return;
        if (collision.CompareTag("Enemy") && ignoreEnemy) return;
        if (collision.CompareTag("NPC") && ignoreNPC) return;

        // Prevents self-hit
        NetworkObject hitObj = collision.GetComponent<NetworkObject>();
        NetworkObject attacker = owner.GetComponent<NetworkObject>();
        if (hitObj == null || attacker == null) return;
        if (hitObj == attacker) return;

        // Don't take Damage if Immune
        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null && buffs.immune.net_IsImmune.Value) return;

        // create a new context for the trigger effects
        SkillContext triggerCtx = context;
        triggerCtx.Target = hitObj;

        // Execute all effects
        foreach (SkillEffect effect in onTriggerEffects) effect.Execute(owner, triggerCtx);
    }
}
