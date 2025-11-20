using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject Tracker;
    [SerializeField] GameObject TrackerM;

    [Header("Prefab")]
    [SerializeField] GameObject Prefab_EventTitle;
    [SerializeField] GameObject Prefab_EventObjective;

    [Header("Trackers")]
    private GameObject activeTracker;
    private Dictionary<ITotemEvent, EventUIEntry> activeEvents = new();

    private struct EventUIEntry
    {
        public GameObject Root;
        public TextMeshProUGUI ObjectiveText;
    }

    private void Start()
    {
        activeTracker = Application.isMobilePlatform ? TrackerM : Tracker;
    }

    public void AddEvent(ITotemEvent evt)
    {
        if (activeEvents.ContainsKey(evt)) return;

        GameObject root = Instantiate(Prefab_EventTitle, activeTracker.transform);
        TextMeshProUGUI title = root.GetComponentInChildren<TextMeshProUGUI>();
        title.text = evt.EventName;
        title.color = Color.orange;

        GameObject obj = Instantiate(Prefab_EventObjective, root.transform);
        TextMeshProUGUI objectiveText = obj.GetComponent<TextMeshProUGUI>();
        objectiveText.text = evt.EventObjective;

        activeEvents[evt] = new EventUIEntry
        {
            Root = root,
            ObjectiveText = objectiveText
        };
    }

    public void UpdateEventObjective(ITotemEvent evt, string newObjective)
    {
        if (!activeEvents.TryGetValue(evt, out EventUIEntry entry))
            return;

        entry.ObjectiveText.text = newObjective;
    }

    public void RemoveEvent(ITotemEvent evt)
    {
        if (!activeEvents.TryGetValue(evt, out EventUIEntry entry))
            return;

        Destroy(entry.Root);
        activeEvents.Remove(evt);
    }
}
