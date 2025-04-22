using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrowdControl : NetworkBehaviour, IKnockbackable
{
    [Header("Knockback")]
    Rigidbody2D rb;
    Vector2 knockBackVelocity;

    [Header("Bools")]
    public bool IsImmobilized;
    public bool IsInterrupted;
    //public bool IsDisarmed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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
        Immobilize(duration);

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

    public void Immobilize(float duration)
    {
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(ImmobilizeDuration(duration));
    }

    IEnumerator ImmobilizeDuration(float duration)
    {
        IsImmobilized = true;

        yield return new WaitForSeconds(duration);

        IsImmobilized = false;
    }

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
}
