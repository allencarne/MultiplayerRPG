using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int AbilityDamage;
    [HideInInspector] public int CharacterDamage;
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public bool IgnoreEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            if (buffs.IsImmune)
            {
                return;
            }
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(AbilityDamage + CharacterDamage, DamageType.Flat, objectThatWasHit);
        }
    }
}