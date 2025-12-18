using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Weakness : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Weakness;

    [HideInInspector] public float weaknessPercent = 0.036f;
    [HideInInspector] public int WeaknessStacks;
    GameObject durationWeaknessUI = null;
    GameObject conditionalWeaknessUI = null;
    float weaknessElapsed = 0f;
    float weaknessTotal = 0f;
    int durationWeaknessStacks = 0;
    int conditionalWeaknessStacks = 0;
    public int TotalWeaknessStacks => durationWeaknessStacks + conditionalWeaknessStacks;
    bool IsWeakened => TotalWeaknessStacks > 0;

    private void Update()
    {
        UpdateWeaknessUI();
    }

    public void StartWeakness(int stacks, float? duration = null)
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
        durationWeaknessStacks += stacks;
        durationWeaknessStacks = Mathf.Min(durationWeaknessStacks, 25);

        UpdateWeaknessUI();
        CalculateDamage();

        if (duration > weaknessTotal - weaknessElapsed)
        {
            BroadcastClientRPC(durationWeaknessStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationWeaknessStacks -= stacks;
        durationWeaknessStacks = Mathf.Max(durationWeaknessStacks, 0);

        UpdateWeaknessUI();
        CalculateDamage();

        BroadcastClientRPC(durationWeaknessStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationWeaknessUI == null)
                durationWeaknessUI = Instantiate(debuff_Weakness, debuffBar.transform);

            durationWeaknessUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                weaknessElapsed = 0f;
                weaknessTotal = remaining;
            }
        }
        else
        {
            if (durationWeaknessUI != null)
            {
                Destroy(durationWeaknessUI);
                durationWeaknessUI = null;
            }

            weaknessElapsed = 0f;
            weaknessTotal = 0f;
        }
    }

    void CalculateDamage()
    {
        float mightMultiplier = buffs.might.TotalMightStacks * buffs.might.mightPercent;
        float weaknessMultiplier = TotalWeaknessStacks * weaknessPercent;
        float multiplier = 1 + mightMultiplier - weaknessMultiplier;

        if (stats != null)
        {
            float damage = stats.BaseDamage * multiplier;
            stats.BaseDamage = Mathf.Max(Mathf.RoundToInt(damage), 1);
        }
    }

    void UpdateWeaknessUI()
    {
        if (weaknessTotal > 0f && durationWeaknessUI != null)
        {
            weaknessElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(weaknessElapsed / weaknessTotal);

            var ui = durationWeaknessUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalWeakness(int stacks)
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
        conditionalWeaknessStacks += stacks;
        conditionalWeaknessStacks = Mathf.Clamp(conditionalWeaknessStacks, 0, 25);

        UpdateWeaknessUI();
        CalculateDamage();
        BroadcastConditionalClientRPC(conditionalWeaknessStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalWeaknessUI == null)
                conditionalWeaknessUI = Instantiate(debuff_Weakness, debuffBar.transform);

            conditionalWeaknessUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalWeaknessUI != null)
            {
                Destroy(conditionalWeaknessUI);
                conditionalWeaknessUI = null;
            }
        }
    }
}
