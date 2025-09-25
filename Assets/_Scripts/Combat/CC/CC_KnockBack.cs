using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CC_KnockBack : NetworkBehaviour, IKnockbackable
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_KnockBack;
    GameObject knockBackInstance;
    public bool IsKnockedBack;
    private float knockBackElapsedTime = 0f;
    private float knockBackTotalDuration = 0f;
    private float localKnockBackElapsed = 0f;
    private float localKnockBackTotal = 0f;

    [Header("Knockback")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CrowdControl crowdControl;
    Vector2 knockBackVelocity;

    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;
    [SerializeField] NPCStateMachine npc;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void KnockBack(Vector2 direction, float amount, float duration)
    {
        direction = direction.normalized;

        if (IsServer)
        {
            Initialize(duration);
            ApplyKnockback(direction, amount, duration);
            ApplyKnockBackClientRPC(direction, amount, duration);
        }
        else
        {
            RequestServerRPC(duration);
        }
    }

    [ServerRpc]
    private void RequestServerRPC(float duration)
    {
        Initialize(duration);
    }

    void Initialize(float duration)
    {
        knockBackTotalDuration += duration;

        if (!IsKnockedBack)
        {
            IsKnockedBack = true;
        }

        float remainingTime = knockBackTotalDuration - knockBackElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isKnockBack, float remainingTime = 0f)
    {
        IsKnockedBack = isKnockBack;

        if (isKnockBack)
        {
            if (knockBackInstance == null)
            {
                knockBackInstance = Instantiate(cc_KnockBack, buffBar.transform);
            }

            localKnockBackElapsed = 0f;
            localKnockBackTotal = remainingTime;
        }
        else
        {
            if (knockBackInstance != null)
            {
                Destroy(knockBackInstance);
            }

            localKnockBackElapsed = 0f;
            localKnockBackTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsKnockedBack)
        {
            knockBackElapsedTime += Time.deltaTime;

            if (knockBackElapsedTime >= knockBackTotalDuration)
            {
                BroadcastClientRPC(false);
                IsKnockedBack = false;

                knockBackElapsedTime = 0f;
                knockBackTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsKnockedBack && localKnockBackTotal > 0f)
        {
            localKnockBackElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localKnockBackElapsed / localKnockBackTotal);

            if (knockBackInstance != null)
            {
                var ui = knockBackInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }

    private void ApplyKnockback(Vector2 direction, float amount, float duration)
    {
        direction = direction.normalized;
        knockBackVelocity = direction * amount;

        if (player != null) player.Hurt();
        if (enemy != null && enemy.currentAbility != null) enemy.InterruptAbility(true);
        if (npc != null) npc.Hurt();

        StartCoroutine(KnockBackDuration(duration));
    }

    [ClientRpc]
    private void ApplyKnockBackClientRPC(Vector2 direction, float amount, float duration)
    {
        if (IsServer) return;
        ApplyKnockback(direction, amount, duration);
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
}
