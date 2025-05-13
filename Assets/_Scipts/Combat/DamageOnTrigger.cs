using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class DamageOnTrigger : NetworkBehaviour
{
    [HideInInspector] public int AbilityDamage;
    [HideInInspector] public int CharacterDamage;
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public bool IgnoreEnemy;
    [SerializeField] GameObject hitSpark;

    public static UnityEvent<NetworkObject> OnDamageDealt = new UnityEvent<NetworkObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

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
            damageable.TakeDamage(AbilityDamage + CharacterDamage, DamageType.Flat, attacker);
            OnDamageDealt?.Invoke(attacker);


            Vector2 pos = collision.ClosestPoint(transform.position);
            Quaternion rot = collision.transform.rotation;

            HitSparkClientRPC(pos, rot);
        }
    }

    [ClientRpc]
    void HitSparkClientRPC(Vector2 position, Quaternion rotation)
    {
        Instantiate(hitSpark, position, rotation);
    }
}