using Unity.Netcode;
using UnityEngine;

public class CC_Disarm : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_Disarm;
    GameObject disarmInstance;
    public bool IsDisarmed;
    private float disarmElapsedTime = 0f;
    private float disarmTotalDuration = 0f;
    private float localDisarmElapsed = 0f;
    private float localDisarmTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartDisarm(float duration)
    {
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
        disarmTotalDuration += duration;

        if (!IsDisarmed)
        {
            IsDisarmed = true;
        }

        float remainingTime = disarmTotalDuration - disarmElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isDisarmed, float remainingTime = 0f)
    {
        IsDisarmed = isDisarmed;

        if (isDisarmed)
        {
            if (disarmInstance == null)
            {
                disarmInstance = Instantiate(cc_Disarm, buffBar.transform);
            }

            localDisarmElapsed = 0f;
            localDisarmTotal = remainingTime;
        }
        else
        {
            if (disarmInstance != null)
            {
                Destroy(disarmInstance);
            }

            localDisarmElapsed = 0f;
            localDisarmTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsDisarmed)
        {
            disarmElapsedTime += Time.deltaTime;

            if (disarmElapsedTime >= disarmTotalDuration)
            {
                BroadcastClientRPC(false);
                IsDisarmed = false;

                disarmElapsedTime = 0f;
                disarmTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsDisarmed && localDisarmTotal > 0f)
        {
            localDisarmElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localDisarmElapsed / localDisarmTotal);

            if (disarmInstance != null)
            {
                var ui = disarmInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }
}
