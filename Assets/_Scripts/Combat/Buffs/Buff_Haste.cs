using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Haste : NetworkBehaviour, IHasteable
{
    [Header("Variables")]
    List<StatModifier> activeModifiers = new List<StatModifier>();
    List<StatModifier> fixedModifiers = new List<StatModifier>();
    float hastePercent = 0.10f;
    int maxStacks = 9;
    float currentRemainingTime = 0f;
    int TotalStacks => activeModifiers.Count + fixedModifiers.Count;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [Header("UI")]
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Haste;
    GameObject hasteUI;

    void Update()
    {
        if (currentRemainingTime > 0)
        {
            currentRemainingTime -= Time.deltaTime;
        }
    }

    public void StartHaste(int stacks, float duration)
    {
        if (!IsOwner) return;

        if (duration < 0)
        {
            StartHasteFixed(stacks);
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
        float multiplier = stats.BaseSpeed * hastePercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = multiplier,
            source = ModSource.Buff
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

        if (hasteUI == null)
        {
            hasteUI = Instantiate(buff_Haste, buffBar.transform);
        }

        StatusEffects se = hasteUI.GetComponent<StatusEffects>();
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
        if (hasteUI != null)
        {
            hasteUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
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
            if (hasteUI != null) Destroy(hasteUI);
        }
    }

    [ServerRpc]
    void DestroeyUIServerRPC(float stacks)
    {
        DestroeyUIClientRPC(stacks);
    }

    public void StartHasteFixed(int stacks)
    {
        if (!IsOwner) return;

        if (stacks < 0)
        {
            if (fixedModifiers.Count < 1) return;

            int stacksToRemove = Mathf.Abs(stacks);
            stacksToRemove = Mathf.Min(stacksToRemove, fixedModifiers.Count);

            for (int i = 0; i < stacksToRemove; i++)
            {
                RemoveStackFixed();
            }

            return;
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            AddStackFixed();
        }
    }

    void AddStackFixed()
    {
        float multiplier = stats.BaseSpeed * hastePercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = multiplier,
            source = ModSource.Buff
        };

        fixedModifiers.Add(mod);
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

    void RemoveStackFixed()
    {
        StatModifier modToRemove = fixedModifiers[0];

        fixedModifiers.Remove(modToRemove);
        stats.RemoveModifier(modToRemove);

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
    void StartFixedUIClientRPC()
    {
        if (hasteUI == null)
        {
            hasteUI = Instantiate(buff_Haste, buffBar.transform);
        }
    }

    [ServerRpc]
    void StartFixedUIServerRPC()
    {
        StartFixedUIClientRPC();
    }

    public void PurgeHaste()
    {
        StopAllCoroutines();

        while (activeModifiers.Count > 0)
        {
            StatModifier mod = activeModifiers[0];
            activeModifiers.Remove(mod);
            stats.RemoveModifier(mod);
        }

        while (fixedModifiers.Count > 0)
        {
            StatModifier mod = fixedModifiers[0];
            fixedModifiers.Remove(mod);
            stats.RemoveModifier(mod);
        }

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
}
