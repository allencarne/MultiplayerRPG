using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int AbilityDamage;
    [HideInInspector] public int CharacterDamage;
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public bool IsBasic;
    [HideInInspector] public bool IsBreakable;
    [SerializeField] GameObject hitSpark;
    [SerializeField] GameObject hitSpark_Special;

    [HideInInspector] public bool IgnorePlayer;
    [HideInInspector] public bool IgnoreEnemy;
    [HideInInspector] public bool IgnoreNPC;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Player") && IgnorePlayer) return;
        if (collision.CompareTag("Enemy") && IgnoreEnemy) return;
        if (collision.CompareTag("NPC") && IgnoreNPC) return;

        Vector2 hitPosition = collision.ClosestPoint(transform.position);
        Vector2 attackerPosition = attacker.transform.position;
        Vector2 direction = (hitPosition - attackerPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        if (collision.CompareTag("Obstacle"))
        {
            if (IsBreakable)
            {
                HitSparkClientRPC(hitPosition, rotation);
                NetworkObject.Despawn(true);
            }
        }

        // Prevent Attacking Self
        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null && objectThatWasHit == attacker) return;

        // Immune
        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null && buffs.immune.IsImmune) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(AbilityDamage + CharacterDamage, DamageType.Flat, attacker);

            // In Combat
            Player player = attacker.GetComponent<Player>();
            if (player != null)
            {
                player.CombatTime = 0;
                player.InCombat = true;
            }

            // Fury
            if (IsBasic)
            {
                PlayerStateMachine stateMachine = attacker.GetComponent<PlayerStateMachine>();
                if (stateMachine != null)
                {
                    Fury fury = stateMachine.GetComponentInChildren<Fury>();
                    if (fury != null)
                    {
                        fury.FuryClientRPC();
                    }
                }
            }

            HitSparkClientRPC(hitPosition, rotation);
        }
    }

    [ClientRpc]
    void HitSparkClientRPC(Vector2 position, Quaternion rotation)
    {
        Instantiate(hitSpark, position, rotation);

        if (hitSpark_Special) Instantiate(hitSpark_Special, position, rotation);
    }
}