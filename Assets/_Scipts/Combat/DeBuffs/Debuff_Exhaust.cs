using System.Collections;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Exhaust : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Exhaust;

    [HideInInspector] public float exhaustPercent = 0.036f;
    [HideInInspector] public int ExhaustStacks;
    GameObject durationExhaustUI = null;
    GameObject conditionalExhaustUI = null;
    float exhaustElapsed = 0f;
    float exhaustTotal = 0f;
    int durationExhaustStacks = 0;
    int conditionalExhaustStacks = 0;
    public int TotalExhaustStacks => durationExhaustStacks + conditionalExhaustStacks;
    bool IsExhausted => TotalExhaustStacks > 0;

    private void Update()
    {
        UpdateExhaustUI();
    }

    public void StartExhaust(int stacks, float? duration = null)
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
        durationExhaustStacks += stacks;
        durationExhaustStacks = Mathf.Min(durationExhaustStacks, 25);

        UpdateExhaustUI();
        CalculateAttackSpeed();

        if (duration > exhaustTotal - exhaustElapsed)
        {
            BroadcastClientRPC(durationExhaustStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationExhaustStacks -= stacks;
        durationExhaustStacks = Mathf.Max(durationExhaustStacks, 0);

        UpdateExhaustUI();
        CalculateAttackSpeed();

        BroadcastClientRPC(durationExhaustStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationExhaustUI == null)
                durationExhaustUI = Instantiate(debuff_Exhaust, debuffBar.transform);

            durationExhaustUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                exhaustElapsed = 0f;
                exhaustTotal = remaining;
            }
        }
        else
        {
            if (durationExhaustUI != null)
            {
                Destroy(durationExhaustUI);
                durationExhaustUI = null;
            }

            exhaustElapsed = 0f;
            exhaustTotal = 0f;
        }
    }

    void CalculateAttackSpeed()
    {
        float swiftnessMultiplier = buffs.swiftness.TotalSwiftnessStacks * buffs.swiftness.swiftnessPercent;
        float exhaustMultiplier = TotalExhaustStacks * exhaustPercent;
        float multiplier = 1 + swiftnessMultiplier - exhaustMultiplier;

        if (player != null)
        {
            float attackspeed = player.BaseAttackSpeed.Value * multiplier;
            player.CurrentAttackSpeed.Value = Mathf.Max(attackspeed, 0.1f);
        }

        if (enemy != null)
        {
            float attackspeed = enemy.BaseAttackSpeed * multiplier;
            enemy.CurrentAttackSpeed = Mathf.Max(attackspeed, 0.1f);
        }
    }

    void UpdateExhaustUI()
    {
        if (exhaustTotal > 0f && durationExhaustUI != null)
        {
            exhaustElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(exhaustElapsed / exhaustTotal);

            var ui = durationExhaustUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalExhaust(int stacks)
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
        conditionalExhaustStacks += stacks;
        conditionalExhaustStacks = Mathf.Clamp(conditionalExhaustStacks, 0, 25);

        UpdateExhaustUI();
        CalculateAttackSpeed();
        BroadcastConditionalClientRPC(conditionalExhaustStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalExhaustUI == null)
                conditionalExhaustUI = Instantiate(debuff_Exhaust, debuffBar.transform);

            conditionalExhaustUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalExhaustUI != null)
            {
                Destroy(conditionalExhaustUI);
                conditionalExhaustUI = null;
            }
        }
    }
}
