using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int AbilityDamage;
    [HideInInspector] public int CharacterDamage;
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public bool IgnoreEnemy;
    [HideInInspector] public bool IsBasic;
    [HideInInspector] public bool IsBreakable;
    [SerializeField] GameObject hitSpark;
    [SerializeField] GameObject hitSpark_Special;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

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

        if (collision.CompareTag("Enemy"))
        {
            if (IgnoreEnemy)
            {
                return;
            }
        }

        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null)
        {
            if (objectThatWasHit == attacker)
            {
                return;
            }
        }

        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null)
        {
            if (buffs.immune.IsImmune)
            {
                return;
            }
        }

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
                var stateMachine = attacker.GetComponent<PlayerStateMachine>();
                if (stateMachine != null)
                {
                    var fury = stateMachine.GetComponentInChildren<Fury>();
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