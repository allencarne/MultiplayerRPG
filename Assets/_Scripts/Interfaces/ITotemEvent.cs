using UnityEngine;

public interface ITotemEvent
{
    string EventName { get; }
    string EventObjective { get; }

    void StartEvent(Transform player);
    void EventSuccess();
    void EventFail();
}
