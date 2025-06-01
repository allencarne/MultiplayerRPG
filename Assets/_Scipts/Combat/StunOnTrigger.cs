using Unity.Netcode;
using UnityEngine;

public class StunOnTrigger : NetworkBehaviour
{
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public float Duration;
    [HideInInspector] public bool IgnoreEnemy;

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

        IStunnable stunnable = collision.GetComponentInChildren<IStunnable>();
        if (stunnable != null)
        {
            stunnable.StartStun(Duration);
        }
    }
}
