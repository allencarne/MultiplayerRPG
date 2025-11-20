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
        Debug.Log(totem.CurrentEvent);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;

        ITotemEvent evt = totem.CurrentEvent;
        if (evt == null) return;

        // If Player leaves active event
        Debug.Log(totem.CurrentEvent);
    }
}
