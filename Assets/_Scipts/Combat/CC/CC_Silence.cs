using Unity.Netcode;
using UnityEngine;

public class CC_Silence : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_Silence;
    GameObject silenceInstance;
    public bool IsSilenced;
    private float silenceElapsedTime = 0f;
    private float silenceTotalDuration = 0f;
    private float localSilenceElapsed = 0f;
    private float localSilenceTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartSilence(float duration)
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
        silenceTotalDuration += duration;

        if (!IsSilenced)
        {
            IsSilenced = true;
        }

        float remainingTime = silenceTotalDuration - silenceElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isSilenced, float remainingTime = 0f)
    {
        IsSilenced = isSilenced;

        if (isSilenced)
        {
            if (silenceInstance == null)
            {
                silenceInstance = Instantiate(cc_Silence, buffBar.transform);
            }

            localSilenceElapsed = 0f;
            localSilenceTotal = remainingTime;
        }
        else
        {
            if (silenceInstance != null)
            {
                Destroy(silenceInstance);
            }

            localSilenceElapsed = 0f;
            localSilenceTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsSilenced)
        {
            silenceElapsedTime += Time.deltaTime;

            if (silenceElapsedTime >= silenceTotalDuration)
            {
                BroadcastClientRPC(false);
                IsSilenced = false;

                silenceElapsedTime = 0f;
                silenceTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsSilenced && localSilenceTotal > 0f)
        {
            localSilenceElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localSilenceElapsed / localSilenceTotal);

            if (silenceInstance != null)
            {
                var ui = silenceInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }
}
