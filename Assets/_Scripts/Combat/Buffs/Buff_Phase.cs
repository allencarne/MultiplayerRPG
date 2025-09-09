using System;
using Unity.Netcode;
using UnityEngine;

public class Buff_Phase : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Phase;
    GameObject phaseInstance;
    public bool IsPhased;
    private float phaseElapsedTime = 0f;
    private float phaseTotalDuration = 0f;
    private float localPhaseElapsed = 0f;
    private float localPhaseTotal = 0f;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartPhase(float duration)
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
        phaseTotalDuration += duration;

        if (!IsPhased)
        {
            IsPhased = true;
        }

        float remainingTime = phaseTotalDuration - phaseElapsedTime;
        BroadcastClientRPC(true, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isPhased, float remainingTime = 0f)
    {
        IsPhased = isPhased;

        Physics2D.IgnoreLayerCollision(6, 6, isPhased); // Player vs Player
        Physics2D.IgnoreLayerCollision(6, 7, isPhased); // Player vs Enemy
        Physics2D.IgnoreLayerCollision(6, 10, isPhased); // Player vs NPC
        Physics2D.IgnoreLayerCollision(7, 7, isPhased); // Enemy vs Enemy
        Physics2D.IgnoreLayerCollision(7, 10, isPhased); // Enemy vs NPC
        Physics2D.IgnoreLayerCollision(10, 10, isPhased); // NPC vs NPC

        if (isPhased)
        {
            if (phaseInstance == null)
            {
                phaseInstance = Instantiate(buff_Phase, buffBar.transform);
            }

            localPhaseElapsed = 0f;
            localPhaseTotal = remainingTime;
        }
        else
        {
            if (phaseInstance != null)
            {
                Destroy(phaseInstance);
            }

            localPhaseElapsed = 0f;
            localPhaseTotal = 0f;
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsPhased)
        {
            phaseElapsedTime += Time.deltaTime;

            if (phaseElapsedTime >= phaseTotalDuration)
            {
                BroadcastClientRPC(false);
                IsPhased = false;

                phaseElapsedTime = 0f;
                phaseTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsPhased && localPhaseTotal > 0f)
        {
            localPhaseElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localPhaseElapsed / localPhaseTotal);

            if (phaseInstance != null)
            {
                var ui = phaseInstance.GetComponent<StatusEffects>();
                if (ui != null) ui.UpdateFill(fill);
            }
        }
    }
}
