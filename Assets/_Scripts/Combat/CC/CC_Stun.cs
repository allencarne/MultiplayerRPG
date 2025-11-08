using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;
    [SerializeField] NPCStateMachine npc;

    //[SerializeField] Animator[] animators;

    private void Update()
    {
        UpdateTimer();
        UpdateUI();
    }

    public void StartStun(float duration)
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

        if (isStunned)
        {
            ApplyStun();

            if (stunInstance == null)
            {
                stunInstance = Instantiate(cc_Stun, buffBar.transform);
            }

            localStunElapsed = 0f;
            localStunTotal = remainingTime;
        }
        else
        {
            if (stunInstance != null)
            {
                Destroy(stunInstance);
            }

            localStunElapsed = 0f;
            localStunTotal = 0f;
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

    void ApplyStun()
    {
        if (player != null) player.Stagger();
        if (enemy != null) enemy.Stagger();
        if (npc != null) npc.Stagger();
    }
}
