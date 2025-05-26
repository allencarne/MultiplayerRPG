using Unity.Netcode;
using UnityEngine;

public class Buff_Immoveable : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Immovable;
    GameObject immovableInstance;
    public bool IsImmovable;
    private float immovableElapsedTime = 0f;
    private float immovableTotalDuration = 0f;
    private float localImmovableElapsed = 0f;
    private float localImmovableTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartImmovable(float duration)
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
        immovableTotalDuration += duration;

        if (!IsImmovable)
        {
            IsImmovable = true;
        }

        float remainingTime = immovableTotalDuration - immovableElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isImmovable, float remainingTime = 0f)
    {
        IsImmovable = isImmovable;

        if (isImmovable)
        {
            if (immovableInstance == null)
            {
                immovableInstance = Instantiate(buff_Immovable, buffBar.transform);
            }

            localImmovableElapsed = 0f;
            localImmovableTotal = remainingTime;
        }
        else
        {
            if (immovableInstance != null)
            {
                Destroy(immovableInstance);
            }

            localImmovableElapsed = 0f;
            localImmovableTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsImmovable)
        {
            immovableElapsedTime += Time.deltaTime;

            if (immovableElapsedTime >= immovableTotalDuration)
            {
                BroadcastClientRPC(false);
                IsImmovable = false;
                immovableElapsedTime = 0f;
                immovableTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsImmovable && localImmovableTotal > 0f)
        {
            localImmovableElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localImmovableElapsed / localImmovableTotal);

            if (immovableInstance != null)
            {
                var ui = immovableInstance.GetComponent<StatusEffects>();
                if (ui != null)
                    ui.UpdateFill(fill);
            }
        }
    }
}
