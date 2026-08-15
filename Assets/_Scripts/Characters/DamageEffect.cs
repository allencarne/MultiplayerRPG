using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Damage Effect")]
public class DamageEffect : ApplyEffect
{
    public float Damage;
    // Add Variable for DamageType

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        Debug.Log("Damage");

        // Get Damageable Component
        IDamageable damageable = target.GetComponent<IDamageable>();

        // Return if we cannot damage
        if (damageable == null) return;

        // Find the Attacker's NetworkObject
        NetworkObject attacker = NetworkManager.Singleton.ConnectedClients[ctx.AttackerId].PlayerObject;

        // Apply Damage to the Attacker
        damageable.TakeDamage(ctx.AttackerDamage + Damage, DamageType.Flat, attacker, target.transform.position);

        // Fury to be removed later
        /*
        if (CanGenerateFury)
        {
            Fury fury = attacker.GetComponentInChildren<Fury>();
            if (fury != null) fury.FuryClientRPC(attacker);
        }
        */
    }
}
