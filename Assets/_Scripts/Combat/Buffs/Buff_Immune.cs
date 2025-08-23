using Unity.Netcode;
using UnityEngine;

public class Buff_Immune : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Immune;
    GameObject immuneInstance;
    public bool IsImmune;
    private float immuneElapsedTime = 0f;
    private float immuneTotalDuration = 0f;
    private float localImmuneElapsed = 0f;
    private float localImmuneTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartImmune(float duration)
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
        immuneTotalDuration += duration;

        if (!IsImmune)
        {
            IsImmune = true;
        }

        float remainingTime = immuneTotalDuration - immuneElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isImmune, float remainingTime = 0f)
    {
        IsImmune = isImmune;

        if (isImmune)
        {
            if (immuneInstance == null)
            {
                immuneInstance = Instantiate(buff_Immune, buffBar.transform);
            }

            localImmuneElapsed = 0f;
            localImmuneTotal = remainingTime;
        }
        else
        {
            if (immuneInstance != null)
            {
                Destroy(immuneInstance);
            }

            localImmuneElapsed = 0f;
            localImmuneTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsImmune)
        {
            immuneElapsedTime += Time.deltaTime;

            if (immuneElapsedTime >= immuneTotalDuration)
            {
                BroadcastClientRPC(false);
                IsImmune = false;
                immuneElapsedTime = 0f;
                immuneTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsImmune && localImmuneTotal > 0f)
        {
            localImmuneElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localImmuneElapsed / localImmuneTotal);

            if (immuneInstance != null)
            {
                var ui = immuneInstance.GetComponent<StatusEffects>();
                if (ui != null)
                    ui.UpdateFill(fill);
            }
        }
    }
}
