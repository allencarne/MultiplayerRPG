using Unity.Netcode;
using UnityEngine;

public class CC_Immobilize : NetworkBehaviour
{
    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject cc_Immob;
    GameObject immobInstance;
    public bool IsImmobilized;
    private float immobElapsedTime = 0f;
    private float immobTotalDuration = 0f;
    private float localImmobElapsed = 0f;
    private float localImmobTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartImmobilize(float duration)
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
        immobTotalDuration += duration;

        if (!IsImmobilized)
        {
            IsImmobilized = true;
        }

        float remainingTime = immobTotalDuration - immobElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isImmobilized, float remainingTime = 0f)
    {
        IsImmobilized = isImmobilized;

        if (isImmobilized)
        {
            if (immobInstance == null)
            {
                immobInstance = Instantiate(cc_Immob, debuffBar.transform);
            }

            localImmobElapsed = 0f;
            localImmobTotal = remainingTime;
        }
        else
        {
            if (immobInstance != null)
            {
                Destroy(immobInstance);
            }

            localImmobElapsed = 0f;
            localImmobTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsImmobilized)
        {
            immobElapsedTime += Time.deltaTime;

            if (immobElapsedTime >= immobTotalDuration)
            {
                BroadcastClientRPC(false);
                IsImmobilized = false;

                immobElapsedTime = 0f;
                immobTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsImmobilized && localImmobTotal > 0f)
        {
            localImmobElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localImmobElapsed / localImmobTotal);

            if (immobInstance != null)
            {
                var ui = immobInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }
}
