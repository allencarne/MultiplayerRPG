using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrowdControl : NetworkBehaviour, IKnockbackable
{
    public CC_Disarm disarm;
    public CC_Silence silence;
    public CC_Immobilize immobilize;

    [Header("Knockback")]
    Rigidbody2D rb;
    Vector2 knockBackVelocity;

    [Header("Bools")]
    public bool IsInterrupted;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    #region Interrupt

    public void Interrupt(float duration)
    {
        StartCoroutine(InterruptDuration(duration));
    }

    IEnumerator InterruptDuration(float duration)
    {
        IsInterrupted = true;

        yield return new WaitForSeconds(duration);

        IsInterrupted = false;
    }

    #endregion

    #region Knockback

    public void KnockBack(Vector2 direction, float amount, float duration)
    {
        if (!IsServer) return;

        direction = direction.normalized;
        ApplyKnockBackClientRpc(direction, amount, duration);
        ApplyKnockback(direction, amount, duration);
    }

    [ClientRpc]
    private void ApplyKnockBackClientRpc(Vector2 direction, float amount, float duration)
    {
        if (IsServer) return;

        ApplyKnockback(direction, amount, duration);
    }

    private void ApplyKnockback(Vector2 direction, float amount, float duration)
    {
        direction = direction.normalized;
        knockBackVelocity = direction * amount;

        Interrupt(duration);
        immobilize.StartImmobilize(duration);

        StartCoroutine(KnockBackDuration(duration));
    }

    IEnumerator KnockBackDuration(float duration)
    {
        float elapsedTime = 0f;
        Vector2 initialVelocity = knockBackVelocity;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            rb.linearVelocity = Vector2.Lerp(initialVelocity, Vector2.zero, t);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    #endregion
}
