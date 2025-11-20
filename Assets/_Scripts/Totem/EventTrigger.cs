using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] Totem totem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;

        ITotemEvent evt = totem.CurrentEvent;
        if (evt == null) return;

        // If Player Enters active event
        tracker.CreateEventEntry(evt.EventName, evt.EventObjective);

        // Subscribe to live updates
        evt.OnObjectiveChanged += tracker.UpdateObjectiveText;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;

        ITotemEvent evt = totem.CurrentEvent;
        if (evt == null) return;

        // Unsubscribe
        evt.OnObjectiveChanged -= tracker.UpdateObjectiveText;

        // Remove UI
        tracker.RemoveEventEntry();
    }
}
