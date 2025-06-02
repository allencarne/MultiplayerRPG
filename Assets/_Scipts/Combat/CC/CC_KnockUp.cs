using System.Collections;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CC_KnockUp : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_KnockUp;
    GameObject knockUpInstance;
    public bool IsKnockedUp;
    private float knockupElapsedTime = 0f;
    private float knockupTotalDuration = 0f;
    private float localKnockUpElapsed = 0f;
    private float localKnockUpTotal = 0f;

    [SerializeField] CrowdControl crowdControl;
    [SerializeField] Transform[] parts;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartKnockUp(float duration)
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            Initialize(duration);
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
        knockupTotalDuration += duration;

        if (!IsKnockedUp)
        {
            IsKnockedUp = true;
        }

        float remainingTime = knockupTotalDuration - knockupElapsedTime;
        BroadcastClientRPC(true, duration, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isKnockedUp, float duration, float remainingTime = 0f)
    {
        IsKnockedUp = isKnockedUp;

        if (isKnockedUp)
        {
            ApplyKnockUp(duration);

            if (knockUpInstance == null)
            {
                knockUpInstance = Instantiate(cc_KnockUp, buffBar.transform);
            }

            localKnockUpElapsed = 0f;
            localKnockUpTotal = remainingTime;
        }
        else
        {
            if (knockUpInstance != null)
            {
                Destroy(knockUpInstance);
            }

            localKnockUpElapsed = 0f;
            localKnockUpTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsKnockedUp)
        {
            knockupElapsedTime += Time.deltaTime;

            if (knockupElapsedTime >= knockupTotalDuration)
            {
                BroadcastClientRPC(false, 0);
                IsKnockedUp = false;

                knockupElapsedTime = 0f;
                knockupTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsKnockedUp && localKnockUpTotal > 0f)
        {
            localKnockUpElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localKnockUpElapsed / localKnockUpTotal);

            if (knockUpInstance != null)
            {
                var ui = knockUpInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }

    void ApplyKnockUp(float duration)
    {
        crowdControl.Interrupt(duration);
        crowdControl.immobilize.StartImmobilize(duration);
        crowdControl.disarm.StartDisarm(duration);
        crowdControl.silence.StartSilence(duration);

        StartCoroutine(KnockUpDuration(duration));
    }

    IEnumerator KnockUpDuration(float duration)
    {
        float half = duration / 2;
        float elapsedTime = 0f;

        while (elapsedTime < half)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].transform.Translate(Vector2.up * Time.deltaTime);
            }
        }

        yield return new WaitForSeconds(half);

        elapsedTime = 0f;
        while (elapsedTime < half)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].transform.Translate(-Vector2.up * Time.deltaTime);
            }
        }
    }
}
