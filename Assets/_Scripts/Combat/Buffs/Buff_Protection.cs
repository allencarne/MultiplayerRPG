using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Protection : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] Enemy enemy;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Protection;

    [HideInInspector] public float protectionPercent = 0.036f;
    [HideInInspector] public int ProtectionStacks;
    GameObject durationProtectionUI = null;
    GameObject conditionalProtectionUI = null;
    float protectionElapsed = 0f;
    float protectionTotal = 0f;
    int durationProtectionStacks = 0;
    int conditionalProtectionStacks = 0;
    bool IsProtected => TotalProtectionStacks > 0;
    public int TotalProtectionStacks => durationProtectionStacks + conditionalProtectionStacks;

    private void Update()
    {
        UpdateProtectionUI();
    }

    public void StartProtection(int stacks, float? duration = null)
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
        durationProtectionStacks += stacks;
        durationProtectionStacks = Mathf.Min(durationProtectionStacks, 25);

        UpdateProtectionUI();
        CalculateArmor();

        if (duration > protectionTotal - protectionElapsed)
        {
            BroadcastClientRPC(durationProtectionStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationProtectionStacks -= stacks;
        durationProtectionStacks = Mathf.Max(durationProtectionStacks, 0);

        UpdateProtectionUI();
        CalculateArmor();

        BroadcastClientRPC(durationProtectionStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationProtectionUI == null)
                durationProtectionUI = Instantiate(buff_Protection, buffBar.transform);

            durationProtectionUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                protectionElapsed = 0f;
                protectionTotal = remaining;
            }
        }
        else
        {
            if (durationProtectionUI != null)
            {
                Destroy(durationProtectionUI);
                durationProtectionUI = null;
            }

            protectionElapsed = 0f;
            protectionTotal = 0f;
        }
    }

    void CalculateArmor()
    {
        float protectionMultiplier = TotalProtectionStacks * protectionPercent;
        float vulnerabilityMultiplier = deBuffs.vulnerability.TotalVulnerabilityStacks * deBuffs.vulnerability.vulnerabilityPercent;
        float multiplier = 1 + protectionMultiplier - vulnerabilityMultiplier;

        if (stats != null) stats.Armor = stats.Armor * multiplier;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor * multiplier;
    }

    void UpdateProtectionUI()
    {
        if (protectionTotal > 0f && durationProtectionUI != null)
        {
            protectionElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(protectionElapsed / protectionTotal);

            var ui = durationProtectionUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalProtection(int stacks)
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
        conditionalProtectionStacks += stacks;
        conditionalProtectionStacks = Mathf.Clamp(conditionalProtectionStacks, 0, 25);

        UpdateProtectionUI();
        CalculateArmor();
        BroadcastConditionalClientRPC(conditionalProtectionStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalProtectionUI == null)
                conditionalProtectionUI = Instantiate(buff_Protection, buffBar.transform);

            conditionalProtectionUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalProtectionUI != null)
            {
                Destroy(conditionalProtectionUI);
                conditionalProtectionUI = null;
            }
        }
    }
}
