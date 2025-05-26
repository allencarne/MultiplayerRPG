using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Might : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Might;

    [HideInInspector] public float mightPercent = 0.036f;
    [HideInInspector] public int MightStacks;
    GameObject durationMightUI = null;
    GameObject conditionalMightUI = null;
    float mightElapsed = 0f;
    float mightTotal = 0f;
    int durationMightStacks = 0;
    int conditionalMightStacks = 0;
    bool IsMightActive => TotalMightStacks > 0;
    public int TotalMightStacks => durationMightStacks + conditionalMightStacks;

    private void Update()
    {
        UpdateMightUI();
    }

    public void StartMight(int stacks, float? duration = null)
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
        durationMightStacks += stacks;
        durationMightStacks = Mathf.Min(durationMightStacks, 25);

        UpdateMightUI();
        CalculateDamage();

        if (duration > mightTotal - mightElapsed)
        {
            BroadcastClientRPC(durationMightStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationMightStacks -= stacks;
        durationMightStacks = Mathf.Max(durationMightStacks, 0);

        UpdateMightUI();
        CalculateDamage();

        BroadcastClientRPC(durationMightStacks);
    }

    [ClientRpc]
    void BroadcastClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationMightUI == null)
                durationMightUI = Instantiate(buff_Might, buffBar.transform);

            durationMightUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                mightElapsed = 0f;
                mightTotal = remaining;
            }
        }
        else
        {
            if (durationMightUI != null)
            {
                Destroy(durationMightUI);
                durationMightUI = null;
            }

            mightElapsed = 0f;
            mightTotal = 0f;
        }
    }

    void CalculateDamage()
    {
        float mightMultiplier = TotalMightStacks * mightPercent;
        float weaknessMultiplier = deBuffs.weakness.TotalWeaknessStacks * deBuffs.weakness.weaknessPercent;
        float multiplier = 1 + mightMultiplier - weaknessMultiplier;

        if (player != null) player.CurrentDamage.Value = Mathf.RoundToInt(player.BaseDamage.Value * multiplier);
        if (enemy != null) enemy.CurrentDamage = Mathf.RoundToInt(enemy.BaseDamage * multiplier);
    }

    void UpdateMightUI()
    {
        if (mightTotal > 0f && durationMightUI != null)
        {
            mightElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(mightElapsed / mightTotal);

            var ui = durationMightUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

    public void StartConditionalMight(int stacks)
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
        conditionalMightStacks += stacks;
        conditionalMightStacks = Mathf.Clamp(conditionalMightStacks, 0, 25);

        UpdateMightUI();
        CalculateDamage();
        BroadcastConditionalClientRPC(conditionalMightStacks);
    }

    [ClientRpc]
    void BroadcastConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalMightUI == null)
                conditionalMightUI = Instantiate(buff_Might, buffBar.transform);

            conditionalMightUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalMightUI != null)
            {
                Destroy(conditionalMightUI);
                conditionalMightUI = null;
            }
        }
    }
}
