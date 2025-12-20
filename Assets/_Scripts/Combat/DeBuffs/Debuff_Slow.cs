using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Slow : NetworkBehaviour, ISlowable
{
    private List<StatModifier> activeModifiers = new List<StatModifier>();
    float slowPercent = 0.10f;
    private int maxStacks = 9;
    public int TotalStacks => activeModifiers.Count;

    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;

    public void StartSlow(int stacks, float duration)
    {
        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

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

        if (TotalStacks == 1 && debuff_Slow != null)
        {
            // Add UI
        }

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);

        if (TotalStacks == 0 && debuff_Slow != null)
        {
            // Remove UI
        }
    }
}
