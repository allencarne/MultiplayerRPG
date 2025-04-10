using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CrowdControl : NetworkBehaviour, IKnockbackable
{
    [Header("Knockback")]
    Rigidbody2D rb;
    Vector2 knockBackVelocity;
    float knockBackDuration;

    [Header("Bools")]
    public bool IsImmobilized;
    public bool IsInterrupted;
    //public bool IsDisarmed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (knockBackDuration > 0)
        {
            rb.linearVelocity = knockBackVelocity;
            knockBackDuration -= Time.fixedDeltaTime;
        }
    }

    public void KnockBack(Vector2 direction, float amount, float duration)
    {
        if (!IsServer) return;

        knockBackVelocity = direction * amount;
        knockBackDuration = duration;

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
        knockBackVelocity = direction * amount;
        knockBackDuration = duration;

        Interrupt(duration);
        Immobilize(duration);

        StartCoroutine(KnockBackDuration(duration));
    }

    IEnumerator KnockBackDuration(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Normalized time
            rb.linearVelocity = Vector2.Lerp(knockBackVelocity, Vector2.zero, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the velocity is exactly zero at the end
        rb.linearVelocity = Vector2.zero;
    }

    public void Immobilize(float duration)
    {
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
