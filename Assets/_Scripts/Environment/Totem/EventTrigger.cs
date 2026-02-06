using Unity.Netcode;
using UnityEngine;

public class EventTrigger : NetworkBehaviour
{
    [SerializeField] Totem totem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;
        if (totem.CurrentEvent == null) return;

        tracker.AddEvent(totem);
        totem.NetEventObjective.OnValueChanged += tracker.NetworkObjectiveUpdated;
        totem.NetEventTime.OnValueChanged += tracker.NetworkTimeUpdated;
        totem.NetEventName.OnValueChanged += tracker.NetworkNameUpdated;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EventTracker tracker = collision.GetComponentInChildren<EventTracker>();
        if (!tracker) return;
        if (totem.CurrentEvent == null) return;

        // Unbind callbacks
        totem.NetEventObjective.OnValueChanged -= tracker.NetworkObjectiveUpdated;
        totem.NetEventTime.OnValueChanged -= tracker.NetworkTimeUpdated;
        totem.NetEventName.OnValueChanged -= tracker.NetworkNameUpdated;

        tracker.RemoveEvent(totem);
    }
}
