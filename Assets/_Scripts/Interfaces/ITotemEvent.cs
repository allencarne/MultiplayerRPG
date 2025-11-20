using UnityEngine;

public interface ITotemEvent
{
    string EventName { get; }
    string EventObjective { get; }

    event System.Action<string> OnObjectiveChanged;

    void StartEvent(Transform player);
    void EventSuccess();
    void EventFail();
}
