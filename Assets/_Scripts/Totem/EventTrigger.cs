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

        tracker.AddEvent(evt);
        evt.OnObjectiveChanged += (obj) => tracker.UpdateEventObjective(evt, obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;

        ITotemEvent evt = totem.CurrentEvent;
        if (evt == null) return;

        evt.OnObjectiveChanged -= (obj) => tracker.UpdateEventObjective(evt, obj);
        tracker.RemoveEvent(evt);
    }
}
