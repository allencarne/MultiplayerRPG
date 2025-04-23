using Unity.Netcode;
using UnityEngine;

public class SlowOnTrigger : MonoBehaviour
{
    [HideInInspector] public int Stacks;
    [HideInInspector] public float Duration;

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

        ISlowable slowable = collision.GetComponent<ISlowable>();

        if (slowable != null)
        {
            slowable.Slow(Stacks, Duration);
        }
    }
}
