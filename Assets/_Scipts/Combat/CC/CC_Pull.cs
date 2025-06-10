using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CC_Pull : NetworkBehaviour, IPullable
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_Pull;
    GameObject pullInstance;
    public bool IsPulled;
    private float pullElapsedTime = 0f;
    private float pullTotalDuration = 0f;
    private float localPullElapsed = 0f;
    private float localPullTotal = 0f;

    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;

    [Header("Knockback")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CrowdControl crowdControl;
    Vector2 pullVelocity;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void Pull(Vector2 direction, float amount, float duration)
    {
        direction = direction.normalized;

        if (IsServer)
        {
            Initialize(duration);
            ApplyPull(direction, amount, duration);
            ApplyPullClientRPC(direction, amount, duration);
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
        pullTotalDuration += duration;

        if (!IsPulled)
        {
            IsPulled = true;
        }

        float remainingTime = pullTotalDuration - pullElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isPulled, float remainingTime = 0f)
    {
        IsPulled = isPulled;

        if (isPulled)
        {
            if (pullInstance == null)
            {
                pullInstance = Instantiate(cc_Pull, buffBar.transform);
            }

            localPullElapsed = 0f;
            localPullTotal = remainingTime;
        }
        else
        {
            if (pullInstance != null)
            {
                Destroy(pullInstance);
            }

            localPullElapsed = 0f;
            localPullTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsPulled)
        {
            pullElapsedTime += Time.deltaTime;

            if (pullElapsedTime >= pullTotalDuration)
            {
                BroadcastClientRPC(false);
                IsPulled = false;

                pullElapsedTime = 0f;
                pullTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsPulled && localPullTotal > 0f)
        {
            localPullElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localPullElapsed / localPullTotal);

            if (pullInstance != null)
            {
                var ui = pullInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }

    private void ApplyPull(Vector2 direction, float amount, float duration)
    {
        direction = direction.normalized;
        pullVelocity = direction * amount;

        if (player != null)
        {

        }

        if (enemy != null)
        {
            enemy.Hurt();
        }

        StartCoroutine(PullDuration(duration));
    }

    [ClientRpc]
    private void ApplyPullClientRPC(Vector2 direction, float amount, float duration)
    {
        if (IsServer) return;

        ApplyPull(direction, amount, duration);
    }

    IEnumerator PullDuration(float duration)
    {
        float elapsedTime = 0f;
        Vector2 initialVelocity = pullVelocity;

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
