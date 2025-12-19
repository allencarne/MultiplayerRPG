using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Buff_Haste : NetworkBehaviour, IHasteable
{
    private List<StatModifier> activeModifiers = new List<StatModifier>();

    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Haste;

    private int maxStacks = 25;
    public int TotalStacks => activeModifiers.Count;


    public void StartHaste(int stacks, float duration)
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
            value = stackValue
        };

        activeModifiers.Add(mod);
        stats.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);
    }
}
