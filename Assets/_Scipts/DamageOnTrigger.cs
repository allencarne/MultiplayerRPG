using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(1, DamageType.Flat);
        }
    }
}
