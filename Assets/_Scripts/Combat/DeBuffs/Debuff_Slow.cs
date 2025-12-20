using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Slow : NetworkBehaviour, ISlowable
{
    [Header("Variables")]
    List<StatModifier> activeModifiers = new List<StatModifier>();
    float slowPercent = 0.10f;
    int maxStacks = 9;
    int TotalStacks => activeModifiers.Count;
    float longestDuration = 0f;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [Header("UI")]
    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;
    GameObject slowUI;

    public void StartSlow(int stacks, float duration)
    {
        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        longestDuration = Mathf.Max(longestDuration, duration);

        for (int i = 0; i < stacksToAdd; i++)
        {
            StartCoroutine(Duration(duration));
        }

        if (slowUI != null)
        {
            StatusEffects se = slowUI.GetComponent<StatusEffects>();
            se.StartUI(longestDuration);
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

        if (TotalStacks == 1)
        {
            slowUI = Instantiate(debuff_Slow, debuffBar.transform);

            StatusEffects se = slowUI.GetComponent<StatusEffects>();
            if (slowUI != null)
            {
                longestDuration = duration;
                se.StartUI(duration);
            }
        }

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);

        if (TotalStacks == 0)
        {
            longestDuration = 0f;
            if (slowUI != null) Destroy(slowUI);
        }
    }
}
