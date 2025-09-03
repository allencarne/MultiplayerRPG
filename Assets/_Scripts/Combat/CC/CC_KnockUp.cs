using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CC_KnockUp : NetworkBehaviour, IKnockupable
{
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject cc_KnockUp;
    GameObject knockUpInstance;
    public bool IsKnockedUp;
    private float knockupElapsedTime = 0f;
    private float knockupTotalDuration = 0f;
    private float localKnockUpElapsed = 0f;
    private float localKnockUpTotal = 0f;

    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;
    [SerializeField] NPCStateMachine npc;

    [SerializeField] CrowdControl crowdControl;
    [SerializeField] Transform[] parts;
    [SerializeField] Canvas canvas;

    Vector3 maxHeight = new Vector3(0, 1f, 0);

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
            ApplyKnockUp();

            if (knockUpInstance == null)
            {
                knockUpInstance = Instantiate(cc_KnockUp, buffBar.transform);

                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].transform.position = parts[i].transform.position + maxHeight;
                    canvas.transform.position = canvas.transform.position + new Vector3(0, .2f, 0);
                }
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

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].transform.position = parts[i].transform.position + -maxHeight;
                canvas.transform.position = canvas.transform.position + new Vector3(0, -.2f, 0);
            }
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

    void ApplyKnockUp()
    {
        if (player != null) player.Hurt();
        if (enemy != null) enemy.Hurt();
        if (npc != null) npc.Hurt();
    }
}
