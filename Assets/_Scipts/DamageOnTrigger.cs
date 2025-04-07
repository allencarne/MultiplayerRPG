using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int Damage;
    [HideInInspector] public NetworkObject attacker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NetworkObject objectThatWasHit = collision.GetComponent<NetworkObject>();
        if (objectThatWasHit != null)
        {
            if (objectThatWasHit == attacker)
            {
                return;
            }
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage, DamageType.Flat, objectThatWasHit);
        }
    }
}