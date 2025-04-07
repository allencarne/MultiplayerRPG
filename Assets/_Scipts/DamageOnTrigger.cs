using Unity.Netcode;
using UnityEngine;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int Damage;
    [HideInInspector] public ulong AttackerClientId;
    [HideInInspector] public NetworkObject attacker;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NetworkObject hitObject = collision.GetComponent<NetworkObject>();
        if (hitObject != null)
        {
            if (hitObject == attacker)
            {
                Debug.Log("OBJECT HIS IS ATTACKER");
                return;
            }
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage, DamageType.Flat, AttackerClientId);
        }
    }
}