using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Swiftness : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Swiftness;

    [HideInInspector] public float swiftnessPercent = 0.036f;
    [HideInInspector] public int SwiftnessStacks;
    GameObject durationSwiftnessUI = null;
    GameObject conditionalSwiftnessUI = null;
    float swiftnessElapsed = 0f;
    float swiftnessTotal = 0f;
    int durationSwiftnessStacks = 0;
    int conditionalSwiftnessStacks = 0;
    bool IsSwift => TotalSwiftnessStacks > 0;
    public int TotalSwiftnessStacks => durationSwiftnessStacks + conditionalSwiftnessStacks;

    private void Update()
    {
        UpdateSwiftnessUI();
    }

    public void StartSwiftness(int stacks, float? duration = null)
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
        durationSwiftnessStacks += stacks;
        durationSwiftnessStacks = Mathf.Min(durationSwiftnessStacks, 25);

        UpdateSwiftnessUI();
        CalculateAttackSpeed();

        if (duration > swiftnessTotal - swiftnessElapsed)
        {
            BroadcastClientRPC(durationSwiftnessStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationSwiftnessStacks -= stacks;
        durationSwiftnessStacks = Mathf.Max(durationSwiftnessStacks, 0);

        UpdateSwiftnessUI();
        CalculateAttackSpeed();

        BroadcastClientRPC(durationSwiftnessStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationSwiftnessUI == null)
                durationSwiftnessUI = Instantiate(buff_Swiftness, buffBar.transform);

            durationSwiftnessUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                swiftnessElapsed = 0f;
                swiftnessTotal = remaining;
            }
        }
        else
        {
            if (durationSwiftnessUI != null)
            {
                Destroy(durationSwiftnessUI);
                durationSwiftnessUI = null;
            }

            swiftnessElapsed = 0f;
            swiftnessTotal = 0f;
        }
    }

    void CalculateAttackSpeed()
    {
        float swiftnessMultiplier = TotalSwiftnessStacks * swiftnessPercent;
        float exhaustMultiplier = deBuffs.exhaust.TotalExhaustStacks * deBuffs.exhaust.exhaustPercent;

        float multiplier = 1 + swiftnessMultiplier - exhaustMultiplier;

        if (stats != null) stats.BaseAS = stats.BaseAS * multiplier;
    }

    void UpdateSwiftnessUI()
    {
        if (swiftnessTotal > 0f && durationSwiftnessUI != null)
        {
            swiftnessElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(swiftnessElapsed / swiftnessTotal);

            var ui = durationSwiftnessUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalSwiftness(int stacks)
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
        conditionalSwiftnessStacks += stacks;
        conditionalSwiftnessStacks = Mathf.Clamp(conditionalSwiftnessStacks, 0, 25);

        UpdateSwiftnessUI();
        CalculateAttackSpeed();
        BroadcastConditionalClientRPC(conditionalSwiftnessStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalSwiftnessUI == null)
                conditionalSwiftnessUI = Instantiate(buff_Swiftness, buffBar.transform);

            conditionalSwiftnessUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalSwiftnessUI != null)
            {
                Destroy(conditionalSwiftnessUI);
                conditionalSwiftnessUI = null;
            }
        }
    }
}
