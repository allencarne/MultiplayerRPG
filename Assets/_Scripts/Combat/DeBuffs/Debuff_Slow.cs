using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Slow : NetworkBehaviour, ISlowable
{
    StatModifier mod = new StatModifier();

    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;

    [HideInInspector] public float slowPercent = 0.036f;
    [HideInInspector] public int SlowStacks;
    GameObject durationSlowUI = null;
    GameObject conditionalSlowUI = null;
    float slowElapsed = 0f;
    float slowTotal = 0f;
    int durationSlowStacks = 0;
    int conditionalSlowStacks = 0;
    public int TotalSlowStacks => durationSlowStacks + conditionalSlowStacks;
    bool IsSlowed => TotalSlowStacks > 0;

    private void Update()
    {
        UpdateSlowUI();
    }

    public void StartSlow(int stacks, float? duration = null)
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            if (duration.HasValue)
                StartCoroutine(Initialize(stacks, duration.Value));
        }
        else
        {
            if (duration.HasValue)
                RequestServerRPC(stacks, duration.Value);
        }
    }

    [ServerRpc]
    void RequestServerRPC(int stacks, float duration)
    {
        StartCoroutine(Initialize(stacks, duration));
    }

    IEnumerator Initialize(int stacks, float duration)
    {
        durationSlowStacks += stacks;
        durationSlowStacks = Mathf.Min(durationSlowStacks, 25);

        UpdateSlowUI();
        CalculateSpeed(false);

        if (duration > slowTotal - slowElapsed)
        {
            BroadcastClientRPC(durationSlowStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationSlowStacks -= stacks;
        durationSlowStacks = Mathf.Max(durationSlowStacks, 0);

        UpdateSlowUI();
        CalculateSpeed(true);

        BroadcastClientRPC(durationSlowStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationSlowUI == null)
                durationSlowUI = Instantiate(debuff_Slow, debuffBar.transform);

            durationSlowUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                slowElapsed = 0f;
                slowTotal = remaining;
            }
        }
        else
        {
            if (durationSlowUI != null)
            {
                Destroy(durationSlowUI);
                durationSlowUI = null;
            }

            slowElapsed = 0f;
            slowTotal = 0f;
        }
    }

    void CalculateSpeed(bool isCompleted)
    {
        if (isCompleted)
        {
            stats.RemoveModifier(mod);
            return;
        }

        mod.value = -SlowStacks;
        mod.statType = StatType.Speed;
        stats.AddModifier(mod);
    }

    void UpdateSlowUI()
    {
        if (slowTotal > 0f && durationSlowUI != null)
        {
            slowElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(slowElapsed / slowTotal);

            StatusEffects ui = durationSlowUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalSlow(int stacks)
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            InitializeConditional(stacks);
        }
        else
        {
            RequestConditionalServerRPC(stacks);
        }
    }

    [ServerRpc]
    void RequestConditionalServerRPC(int stacks)
    {
        InitializeConditional(stacks);
    }

    void InitializeConditional(int stacks)
    {
        conditionalSlowStacks += stacks;
        conditionalSlowStacks = Mathf.Clamp(conditionalSlowStacks, 0, 25);

        UpdateSlowUI();
        CalculateSpeed(false);
        BroadcastConditionalClientRPC(conditionalSlowStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalSlowUI == null)
                conditionalSlowUI = Instantiate(debuff_Slow, debuffBar.transform);

            conditionalSlowUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalSlowUI != null)
            {
                Destroy(conditionalSlowUI);
                conditionalSlowUI = null;
            }
        }
    }
}
