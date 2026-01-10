using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Haste : NetworkBehaviour, IHasteable
{
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject ParticlePrefab;
    GameObject particleInstance;

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
        if (durationModifiers.Count == 0) return;

        if (Time.time >= remainingTime)
        {
            ExpireStack();
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

        remainingTime = Time.time + duration;

        if (IsServer)
        {
            StartUIClientRPC(duration);
        }
        else
        {
            StartUIServerRPC(duration);
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            AddStack(false);
        }
    }

    void AddStack(bool isFixed)
    {
        float multiplier = stats.BaseSpeed * stackPercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = multiplier,
            source = ModSource.Buff
        };

        if (isFixed)
        {
            fixedModifiers.Add(mod);
        }
        else
        {
            durationModifiers.Add(mod);
        }
        stats.AddModifier(mod);

        if (IsServer)
        {
            if (isFixed) StartFixedUIClientRPC();
            UpdateUIClientRPC(TotalStacks);
        }
        else
        {
            if (isFixed) StartFixedUIServerRPC();
            UpdateUIServerRPC(TotalStacks);
        }
    }

    void ExpireStack()
    {
        foreach (StatModifier mod in durationModifiers)
        {
            stats.RemoveModifier(mod);
        }

        durationModifiers.Clear();

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

    void StartHasteFixed(int stacks)
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
            AddStack(true);
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

    public void PurgeHaste()
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

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }

        if (particleInstance == null)
        {
            particleInstance = Instantiate(ParticlePrefab, parentTransform);
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
            if (particleInstance != null) Destroy(particleInstance);
        }
    }

    [ServerRpc]
    void DestroyUIServerRPC(float stacks)
    {
        DestroyUIClientRPC(stacks);
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
}
