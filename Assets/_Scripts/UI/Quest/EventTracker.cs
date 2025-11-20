using TMPro;
using UnityEngine;

public class EventTracker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject Tracker;
    [SerializeField] GameObject TrackerM;
    private GameObject activeTracker;
    bool isMobile;

    [Header("Prefab")]
    [SerializeField] GameObject Prefab_EventTitle;
    [SerializeField] GameObject Prefab_EventObjective;

    private void Start()
    {
        isMobile = Application.isMobilePlatform;
        activeTracker = isMobile ? TrackerM : Tracker;
    }

    public void CreateEventEntry(string eventName, string eventObjective)
    {
        GameObject eventUI = Instantiate(Prefab_EventTitle, activeTracker.transform);

        SetEventTitle(eventUI, eventName);
        CreateObjectiveEntry(eventUI.transform, eventObjective);
    }

    private void SetEventTitle(GameObject eventUI, string questName)
    {
        TextMeshProUGUI questTitle = eventUI.GetComponentInChildren<TextMeshProUGUI>();
        if (questTitle != null)
        {
            questTitle.text = questName;
        }
    }

    private void CreateObjectiveEntry(Transform parent, string eventObjective)
    {
        GameObject objectiveText = Instantiate(Prefab_EventObjective, parent);
        TextMeshProUGUI text = objectiveText.GetComponent<TextMeshProUGUI>();

        if (text != null)
        {
            text.text = eventObjective;
        }
    }
}
