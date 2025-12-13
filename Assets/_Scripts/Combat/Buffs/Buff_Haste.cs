using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Haste : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Haste;

    [HideInInspector] public float hastePercent = 0.036f;
    [HideInInspector] public int HasteStacks;
    GameObject durationHasteUI = null;
    GameObject conditionalHasteUI = null;
    float hasteElapsed = 0f;
    float hasteTotal = 0f;
    int durationHasteStacks = 0;
    int conditionalHasteStacks = 0;
    bool IsHasted => TotalHasteStacks > 0;
    public int TotalHasteStacks => durationHasteStacks + conditionalHasteStacks;

    private void Update()
    {
        UpdateHasteUI();
    }

    public void StartHaste(int stacks, float? duration = null)
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
        durationHasteStacks += stacks;
        durationHasteStacks = Mathf.Min(durationHasteStacks, 25);

        UpdateHasteUI();
        CalculateSpeed();

        if (duration > hasteTotal - hasteElapsed)
        {
            BroadcastClientRPC(durationHasteStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationHasteStacks -= stacks;
        durationHasteStacks = Mathf.Max(durationHasteStacks, 0);

        UpdateHasteUI();
        CalculateSpeed();

        BroadcastClientRPC(durationHasteStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationHasteUI == null)
                durationHasteUI = Instantiate(buff_Haste, buffBar.transform);

            durationHasteUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                hasteElapsed = 0f;
                hasteTotal = remaining;
            }
        }
        else
        {
            if (durationHasteUI != null)
            {
                Destroy(durationHasteUI);
                durationHasteUI = null;
            }

            hasteElapsed = 0f;
            hasteTotal = 0f;
        }
    }

    void CalculateSpeed()
    {
        float hasteMultiplier = TotalHasteStacks * hastePercent;
        float slowMultiplier = deBuffs.slow.TotalSlowStacks * deBuffs.slow.slowPercent;
        float multiplier = 1 + hasteMultiplier - slowMultiplier;

        if (stats != null) stats.Speed = stats.Speed * multiplier;
    }

    void UpdateHasteUI()
    {
        if (hasteTotal > 0f && durationHasteUI != null)
        {
            hasteElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(hasteElapsed / hasteTotal);

            var ui = durationHasteUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalHaste(int stacks)
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
        conditionalHasteStacks += stacks;
        conditionalHasteStacks = Mathf.Clamp(conditionalHasteStacks, 0, 25);

        UpdateHasteUI();
        CalculateSpeed();
        BroadcastConditionalClientRPC(conditionalHasteStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalHasteUI == null)
                conditionalHasteUI = Instantiate(buff_Haste, buffBar.transform);

            conditionalHasteUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalHasteUI != null)
            {
                Destroy(conditionalHasteUI);
                conditionalHasteUI = null;
            }
        }
    }
}
