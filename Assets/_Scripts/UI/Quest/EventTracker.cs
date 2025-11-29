using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject Tracker;
    [SerializeField] GameObject TrackerM;
    [SerializeField] GameObject TrackerPrefab;

    [Header("Trackers")]
    private GameObject activeTracker;

    private Dictionary<Totem, EventUIEntry> activeEvents = new();
    private struct EventUIEntry
    {
        public GameObject Root;
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI ObjectiveText;
        public TextMeshProUGUI TimerText;
    }

    private void Start()
    {
        activeTracker = Application.isMobilePlatform ? TrackerM : Tracker;
    }

    public void AddEvent(Totem totem)
    {
        if (activeEvents.ContainsKey(totem)) return;

        GameObject root = Instantiate(TrackerPrefab, activeTracker.transform);

        TextMeshProUGUI[] texts = root.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI titleText = texts[0];
        TextMeshProUGUI objectiveText = texts[1];
        TextMeshProUGUI timerText = texts[2];

        titleText.text = totem.NetEventName.Value.ToString();
        objectiveText.text = totem.NetEventObjective.Value.ToString();
        timerText.text = $"Event Ends In: {Mathf.CeilToInt(totem.NetEventTime.Value)}s";

        activeEvents[totem] = new EventUIEntry
        {
            Root = root,
            TitleText = titleText,
            ObjectiveText = objectiveText,
            TimerText = timerText
        };
    }

    public void RemoveEvent(Totem totem)
    {
        if (!activeEvents.TryGetValue(totem, out EventUIEntry entry))
            return;

        Destroy(entry.Root);
        activeEvents.Remove(totem);
    }

    public void NetworkObjectiveUpdated(FixedString64Bytes previous, FixedString64Bytes current)
    {
        foreach (var kvp in activeEvents)
        {
            Totem totem = kvp.Key;
            kvp.Value.ObjectiveText.text = totem.NetEventObjective.Value.ToString();
        }
    }

    public void NetworkTimeUpdated(float prev, float curr)
    {
        foreach (var kvp in activeEvents)
        {
            Totem totem = kvp.Key;
            kvp.Value.TimerText.text = $"Event Ends In: {Mathf.CeilToInt(totem.NetEventTime.Value)}s";
        }
    }

    public void NetworkNameUpdated(FixedString64Bytes previous, FixedString64Bytes current)
    {
        foreach (var kvp in activeEvents)
        {
            Totem totem = kvp.Key;
            kvp.Value.TitleText.text = totem.NetEventName.Value.ToString();
        }
    }
}
