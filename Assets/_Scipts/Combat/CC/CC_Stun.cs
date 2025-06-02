using Unity.Netcode;
using UnityEngine;

public class CC_Stun : NetworkBehaviour, IStunnable
{
    [SerializeField] CrowdControl crowdControl;
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_Stun;
    GameObject stunInstance;
    public bool IsStunned;
    private float stunElapsedTime = 0f;
    private float stunTotalDuration = 0f;
    private float localStunElapsed = 0f;
    private float localStunTotal = 0f;

    [SerializeField] Animator[] animators;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartStun(float duration)
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
        stunTotalDuration += duration;

        if (!IsStunned)
        {
            IsStunned = true;
        }

        float remainingTime = stunTotalDuration - stunElapsedTime;
        BroadcastClientRPC(true, duration, remainingTime);
    }

    [ClientRpc]
    private void BroadcastClientRPC(bool isStunned, float duration, float remainingTime = 0f)
    {
        IsStunned = isStunned;

        crowdControl.Interrupt(duration);
        crowdControl.immobilize.StartImmobilize(duration);
        crowdControl.disarm.StartDisarm(duration);
        crowdControl.silence.StartSilence(duration);

        if (isStunned)
        {
            if (stunInstance == null)
            {
                stunInstance = Instantiate(cc_Stun, buffBar.transform);
            }

            localStunElapsed = 0f;
            localStunTotal = remainingTime;

            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].speed = 0f;
            }
        }
        else
        {
            if (stunInstance != null)
            {
                Destroy(stunInstance);
            }

            localStunElapsed = 0f;
            localStunTotal = 0f;

            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].speed = 1f;
            }
        }
    }

    void UpdateTimer()
    {
        if (IsServer && IsStunned)
        {
            stunElapsedTime += Time.deltaTime;

            if (stunElapsedTime >= stunTotalDuration)
            {
                BroadcastClientRPC(false, 0);
                IsStunned = false;

                stunElapsedTime = 0f;
                stunTotalDuration = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (IsStunned && localStunTotal > 0f)
        {
            localStunElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localStunElapsed / localStunTotal);

            if (stunInstance != null)
            {
                var ui = stunInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }
}
