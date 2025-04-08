using Unity.Netcode;
using UnityEngine;

public class KnockbackOnTrigger : MonoBehaviour
{
    [HideInInspector] public NetworkObject attacker;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public float Amount;

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

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            knockbackable.KnockBack(direction, Amount);
        }
    }
}
