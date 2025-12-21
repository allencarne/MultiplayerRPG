using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Slow : NetworkBehaviour, ISlowable
{
    [Header("Variables")]
    List<StatModifier> activeModifiers = new List<StatModifier>();
    float slowPercent = 0.10f;
    int maxStacks = 9;
    float currentRemainingTime = 0f;
    int TotalStacks => activeModifiers.Count;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [Header("UI")]
    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;
    GameObject slowUI;

    void Update()
    {
        if (currentRemainingTime > 0)
        {
            currentRemainingTime -= Time.deltaTime;
        }
    }

    public void StartSlow(int stacks, float duration)
    {
        if (!IsOwner) return;

        if (duration < 0)
        {
            StartSlowFixed(stacks);
            return;
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        if (duration > currentRemainingTime)
        {
            if (IsServer)
            {
                StartUIClientRPC(duration);
            }
            else
            {
                StartUIServerRPC(duration);
            }
        }

        for (int i = 0; i < stacksToAdd; i++)
        {
            StartCoroutine(Duration(duration));
        }
    }

    IEnumerator Duration(float duration)
    {
        float multiplier = stats.BaseSpeed * slowPercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = -multiplier,
            source = ModSource.Debuff
        };

        activeModifiers.Add(mod);
        stats.AddModifier(mod);

        if (IsServer)
        {
            UpdateUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
        }

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);

        if (IsServer)
        {
            UpdateUIClientRPC(TotalStacks);
            DestroeyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroeyUIServerRPC(TotalStacks);
        }
    }

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        currentRemainingTime = duration;

        if (slowUI == null)
        {
            slowUI = Instantiate(debuff_Slow, debuffBar.transform);
        }

        StatusEffects se = slowUI.GetComponent<StatusEffects>();
        se.StartUI(duration);
    }

    [ServerRpc]
    void StartUIServerRPC(float duration)
    {
        StartUIClientRPC(duration);
    }

    [ClientRpc]
    void UpdateUIClientRPC(float stacks)
    {
        if (slowUI != null)
        {
            slowUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
    }

    [ServerRpc]
    void UpdateUIServerRPC(float stacks)
    {
        UpdateUIClientRPC(stacks);
    }

    [ClientRpc]
    void DestroeyUIClientRPC(float stacks)
    {
        if (stacks == 0)
        {
            if (slowUI != null) Destroy(slowUI);
        }
    }

    [ServerRpc]
    void DestroeyUIServerRPC(float stacks)
    {
        DestroeyUIClientRPC(stacks);
    }

    public void StartSlowFixed(int stacks)
    {
        if (!IsOwner) return;

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            StackFixed();
        }
    }

    void StackFixed()
    {
        float multiplier = stats.BaseSpeed * slowPercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = -multiplier,
            source = ModSource.Debuff
        };

        activeModifiers.Add(mod);
        stats.AddModifier(mod);


        if (IsServer)
        {
            StartFixedUIClientRPC();
            UpdateUIClientRPC(TotalStacks);
        }
        else
        {
            StartFixedUIServerRPC();
            UpdateUIServerRPC(TotalStacks);
        }
    }

    [ClientRpc]
    void StartFixedUIClientRPC()
    {
        if (slowUI == null)
        {
            slowUI = Instantiate(debuff_Slow, debuffBar.transform);
        }
    }

    [ServerRpc]
    void StartFixedUIServerRPC()
    {
        StartFixedUIClientRPC();
    }
}
