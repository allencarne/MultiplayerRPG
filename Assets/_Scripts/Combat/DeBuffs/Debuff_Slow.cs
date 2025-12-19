using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Debuff_Slow : NetworkBehaviour, ISlowable
{
    private List<StatModifier> activeModifiers = new List<StatModifier>();

    [SerializeField] CharacterStats stats;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;

    private int maxStacks = 25;
    public int TotalStacks => activeModifiers.Count;

    public void StartSlow(int stacks, float duration)
    {
        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            StartCoroutine(Duration(1, duration));
        }
    }

    IEnumerator Duration(int stackValue, float duration)
    {
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = -stackValue
        };

        activeModifiers.Add(mod);
        stats.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);
    }
}
