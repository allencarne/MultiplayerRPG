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

    public void StartSlow(int stacks, float duration)
    {
        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        if (duration > currentRemainingTime)
        {
            currentRemainingTime = duration;

            if (slowUI == null)
            {
                slowUI = Instantiate(debuff_Slow, debuffBar.transform);
            }

            StatusEffects se = slowUI.GetComponent<StatusEffects>();
            se.StartUI(duration);
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

        UpdateStackText();

        yield return new WaitForSeconds(duration);

        activeModifiers.Remove(mod);
        stats.RemoveModifier(mod);

        UpdateStackText();

        if (TotalStacks == 0)
        {
            if (slowUI != null) Destroy(slowUI);
        }
    }

    void UpdateStackText()
    {
        if (slowUI != null)
        {
            slowUI.GetComponentInChildren<TextMeshProUGUI>().text = TotalStacks.ToString();
        }
    }

    void Update()
    {
        if (currentRemainingTime > 0)
        {
            currentRemainingTime -= Time.deltaTime;
        }
    }
}
