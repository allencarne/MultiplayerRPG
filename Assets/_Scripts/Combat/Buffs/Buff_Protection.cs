using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Protection : NetworkBehaviour, IProtectionable
{
    [Header("Variables")]
    List<StatModifier> durationModifiers = new List<StatModifier>();
    List<StatModifier> fixedModifiers = new List<StatModifier>();
    float stackPercent = 0.10f;
    int maxStacks = 9;
    float remainingTime = 0f;
    int TotalStacks => durationModifiers.Count + fixedModifiers.Count;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject UI_Bar;
    [SerializeField] GameObject UI_Prefab;
    GameObject UI_Instance;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
    }

    public void StartProtection(int stacks, float duration)
    {
        if (!IsOwner) return;

        if (duration < 0)
        {
            StartProtectionFixed(stacks);
            return;
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        if (duration > remainingTime)
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
        float multiplier = stats.BaseSpeed * stackPercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Armor,
            value = multiplier,
            source = ModSource.Buff
        };

        durationModifiers.Add(mod);
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

        durationModifiers.Remove(mod);
        stats.RemoveModifier(mod);

        if (IsServer)
        {
            UpdateUIClientRPC(TotalStacks);
            DestroyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroyUIServerRPC(TotalStacks);
        }
    }

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        remainingTime = duration;

        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }

        StatusEffects se = UI_Instance.GetComponent<StatusEffects>();
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
        if (UI_Instance != null)
        {
            UI_Instance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
    }

    [ServerRpc]
    void UpdateUIServerRPC(float stacks)
    {
        UpdateUIClientRPC(stacks);
    }

    [ClientRpc]
    void DestroyUIClientRPC(float stacks)
    {
        if (stacks == 0)
        {
            if (UI_Instance != null) Destroy(UI_Instance);
        }
    }

    [ServerRpc]
    void DestroyUIServerRPC(float stacks)
    {
        DestroyUIClientRPC(stacks);
    }

    public void StartProtectionFixed(int stacks)
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
        float multiplier = stats.BaseSpeed * stackPercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Armor,
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
            DestroyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroyUIServerRPC(TotalStacks);
        }
    }

    [ClientRpc]
    void StartFixedUIClientRPC()
    {
        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }
    }

    [ServerRpc]
    void StartFixedUIServerRPC()
    {
        StartFixedUIClientRPC();
    }

    public void PurgeProtection()
    {
        StopAllCoroutines();

        while (durationModifiers.Count > 0)
        {
            StatModifier mod = durationModifiers[0];
            durationModifiers.Remove(mod);
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
            DestroyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroyUIServerRPC(TotalStacks);
        }
    }
}
