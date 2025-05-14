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
    [SerializeField] GameObject hitSpark_Special;

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

            Vector2 hitPosition = collision.ClosestPoint(transform.position);
            Vector2 attackerPosition = attacker.transform.position;

            Vector2 direction = (hitPosition - attackerPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

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