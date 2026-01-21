using System;
using TMPro;
using UnityEngine;

public class WorldMapLog : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [SerializeField] TextMeshProUGUI statueProgress;
    [SerializeField] TextMeshProUGUI chestProgress;

    int maxBeachStatue = 5;
    int maxBeachChests = 9;

    private void OnEnable()
    {
        UpdateChestProgress();
        UpdateStatueProgress();
    }

    void UpdateChestProgress()
    {
        int characterNumber = stats.net_CharacterSlot.Value;
        int completedChests = 0;

        for (int i = 0; i < maxBeachChests; i++)
        {
            if (Check(characterNumber, "Beach", "Chest", i))
            {
                completedChests++;
            }
        }

        chestProgress.text = $"{completedChests}/{maxBeachChests}";
    }

    void UpdateStatueProgress()
    {
        int characterNumber = stats.net_CharacterSlot.Value;
        int completedStatues = 0;

        for (int i = 0; i < maxBeachStatue; i++)
        {
            if (Check(characterNumber, "Beach", "Statue", i))
            {
                completedStatues++;
            }
        }

        statueProgress.text = $"{completedStatues}/{maxBeachStatue}";
    }

    bool Check(int characterNumber, string area, string type, int index)
    {
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_{type}_{index}", "Incomplete");
        return status == "Completed";
    }
}
