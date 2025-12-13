using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Alacrity : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Alacrity;

    [HideInInspector] public float alacrityPercent = 0.036f;
    [HideInInspector] public int AlacrityStacks;
    GameObject durationAlacrityUI = null;
    GameObject conditionalAlacrityUI = null;
    float alacrityElapsed = 0f;
    float alacrityTotal = 0f;
    int durationAlacrityStacks = 0;
    int conditionalAlacrityStacks = 0;
    bool IsAlacrityActive => TotalAlacrityStacks > 0;
    public int TotalAlacrityStacks => durationAlacrityStacks + conditionalAlacrityStacks;

    private void Update()
    {
        UpdateAlacrityUI();
    }

    public void StartAlacrity(int stacks, float? duration = null)
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
        durationAlacrityStacks += stacks;
        durationAlacrityStacks = Mathf.Min(durationAlacrityStacks, 25);

        UpdateAlacrityUI();
        CalculateCooldown();

        if (duration > alacrityTotal - alacrityElapsed)
        {
            BroadcastClientRPC(durationAlacrityStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationAlacrityStacks -= stacks;
        durationAlacrityStacks = Mathf.Max(durationAlacrityStacks, 0);

        UpdateAlacrityUI();
        CalculateCooldown();

        BroadcastClientRPC(durationAlacrityStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationAlacrityUI == null)
                durationAlacrityUI = Instantiate(buff_Alacrity, buffBar.transform);

            durationAlacrityUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                alacrityElapsed = 0f;
                alacrityTotal = remaining;
            }
        }
        else
        {
            if (durationAlacrityUI != null)
            {
                Destroy(durationAlacrityUI);
                durationAlacrityUI = null;
            }

            alacrityElapsed = 0f;
            alacrityTotal = 0f;
        }
    }

    void CalculateCooldown()
    {
        float alacrityMultiplier = TotalAlacrityStacks * alacrityPercent;
        float impedeMultiplier = deBuffs.impede.TotalImpedeStacks * deBuffs.impede.impedePercent;
        float multiplier = 1f + alacrityMultiplier - impedeMultiplier;

        if (stats != null) stats.CoolDownReduction = stats.CoolDownReduction * multiplier;
    }

    void UpdateAlacrityUI()
    {
        if (alacrityTotal > 0f && durationAlacrityUI != null)
        {
            alacrityElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(alacrityElapsed / alacrityTotal);

            var ui = durationAlacrityUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalAlacrity(int stacks)
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
        conditionalAlacrityStacks += stacks;
        conditionalAlacrityStacks = Mathf.Clamp(conditionalAlacrityStacks, 0, 25);

        UpdateAlacrityUI();
        CalculateCooldown();
        BroadcastConditionalClientRPC(conditionalAlacrityStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalAlacrityUI == null)
                conditionalAlacrityUI = Instantiate(buff_Alacrity, buffBar.transform);

            conditionalAlacrityUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalAlacrityUI != null)
            {
                Destroy(conditionalAlacrityUI);
                conditionalAlacrityUI = null;
            }
        }
    }
}
