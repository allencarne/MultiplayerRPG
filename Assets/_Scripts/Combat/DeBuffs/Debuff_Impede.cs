using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Impede : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Impede;

    [HideInInspector] public float impedePercent = 0.036f;
    [HideInInspector] public int ImpedeStacks;
    GameObject durationImpedeUI = null;
    GameObject conditionalImpedeUI = null;
    float impedeElapsed = 0f;
    float impedeTotal = 0f;
    int durationImpedeStacks = 0;
    int conditionalImpedeStacks = 0;
    public int TotalImpedeStacks => durationImpedeStacks + conditionalImpedeStacks;
    bool IsImpeded => TotalImpedeStacks > 0;

    private void Update()
    {
        UpdateImpedeUI();
    }

    public void StartImpede(int stacks, float? duration = null)
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
        durationImpedeStacks += stacks;
        durationImpedeStacks = Mathf.Min(durationImpedeStacks, 25);

        UpdateImpedeUI();
        CalculateCooldown();

        if (duration > impedeTotal - impedeElapsed)
        {
            BroadcastClientRPC(durationImpedeStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationImpedeStacks -= stacks;
        durationImpedeStacks = Mathf.Max(durationImpedeStacks, 0);

        UpdateImpedeUI();
        CalculateCooldown();

        BroadcastClientRPC(durationImpedeStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationImpedeUI == null)
                durationImpedeUI = Instantiate(debuff_Impede, debuffBar.transform);

            durationImpedeUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                impedeElapsed = 0f;
                impedeTotal = remaining;
            }
        }
        else
        {
            if (durationImpedeUI != null)
            {
                Destroy(durationImpedeUI);
                durationImpedeUI = null;
            }

            impedeElapsed = 0f;
            impedeTotal = 0f;
        }
    }

    void CalculateCooldown()
    {
        float alacrityMultiplier = buffs.alacrity.TotalAlacrityStacks * buffs.alacrity.alacrityPercent;
        float impedeMultiplier = TotalImpedeStacks * impedePercent;
        float multiplier = 1 + alacrityMultiplier - impedeMultiplier;

        if (stats != null)
        {
            float cdr = stats.CoolDownReduction * multiplier;
            stats.CoolDownReduction = Mathf.Max(cdr, 0.1f);
        }
    }

    void UpdateImpedeUI()
    {
        if (impedeTotal > 0f && durationImpedeUI != null)
        {
            impedeElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(impedeElapsed / impedeTotal);

            var ui = durationImpedeUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalImpede(int stacks)
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
        conditionalImpedeStacks += stacks;
        conditionalImpedeStacks = Mathf.Clamp(conditionalImpedeStacks, 0, 25);

        UpdateImpedeUI();
        CalculateCooldown();
        BroadcastConditionalClientRPC(conditionalImpedeStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalImpedeUI == null)
                conditionalImpedeUI = Instantiate(debuff_Impede, debuffBar.transform);

            conditionalImpedeUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalImpedeUI != null)
            {
                Destroy(conditionalImpedeUI);
                conditionalImpedeUI = null;
            }
        }
    }
}
