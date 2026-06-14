using TMPro;
using UnityEngine;

public class WorldMapLog : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    [SerializeField] TextMeshProUGUI statueProgress;
    [SerializeField] TextMeshProUGUI chestProgress;
    [SerializeField] TextMeshProUGUI questProgress;

    int maxBeachStatue = 5;
    int maxBeachChests = 9;
    [SerializeField] QuestList questList;

    private void OnEnable()
    {
        UpdateChestProgress();
        UpdateStatueProgress();
        UpdateQuestProgress();
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

    void UpdateQuestProgress()
    {
        int characterNumber = stats.net_CharacterSlot.Value;
        string prefix = $"Character{characterNumber}_";
        int completedQuests = 0;

        foreach (Quest quest in questList.QuestDatabase)
        {
            if (quest == null) continue;

            string stateKey = $"{prefix}{quest.QuestID}_State";
            if (!PlayerPrefs.HasKey(stateKey)) continue;

            QuestState state = (QuestState)PlayerPrefs.GetInt(stateKey);
            if (state == QuestState.Completed)
                completedQuests++;
        }

        questProgress.text = $"{completedQuests}/{questList.QuestDatabase.Length}";
    }
}
