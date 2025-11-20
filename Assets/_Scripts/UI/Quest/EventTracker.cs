using TMPro;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject Tracker;
    [SerializeField] GameObject TrackerM;
    private GameObject activeTracker;

    [Header("Prefab")]
    [SerializeField] GameObject Prefab_EventTitle;
    [SerializeField] GameObject Prefab_EventObjective;

    GameObject uiRoot;
    TextMeshProUGUI objectiveText;

    private void Start()
    {
        activeTracker = Application.isMobilePlatform ? TrackerM : Tracker;
    }

    public void CreateEventEntry(string eventName, string eventObjective)
    {
        // Root
        uiRoot = Instantiate(Prefab_EventTitle, activeTracker.transform);

        // Title
        TextMeshProUGUI title = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
        title.text = eventName;

        // Objective
        GameObject objectiveObj = Instantiate(Prefab_EventObjective, uiRoot.transform);
        objectiveText = objectiveObj.GetComponent<TextMeshProUGUI>();
        objectiveText.text = eventObjective;
    }

    public void UpdateObjectiveText(string newObjective)
    {
        if (objectiveText != null)
            objectiveText.text = newObjective;
    }

    public void RemoveEventEntry()
    {
        if (uiRoot != null)
            Destroy(uiRoot);

        uiRoot = null;
        objectiveText = null;
    }
}
