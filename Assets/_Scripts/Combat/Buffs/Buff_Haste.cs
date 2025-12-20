using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Buff_Haste : NetworkBehaviour, IHasteable
{
    [Header("Variables")]
    List<StatModifier> activeModifiers = new List<StatModifier>();
    float hastePercent = 0.10f;
    int maxStacks = 9;
    int TotalStacks => activeModifiers.Count;
    float activeTime = 0f;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] DeBuffs deBuffs;

    [Header("UI")]
    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Haste;
    GameObject hasteUI;


    public void StartHaste(int stacks, float duration)
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
        float multiplier = stats.BaseSpeed * hastePercent;
        StatModifier mod = new StatModifier
        {
            statType = StatType.Speed,
            value = multiplier,
            source = ModSource.Buff
        };

        activeModifiers.Add(mod);
        stats.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);
    }
}
