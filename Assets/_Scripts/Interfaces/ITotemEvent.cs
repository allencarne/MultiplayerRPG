using UnityEngine;

public interface ITotemEvent
{
    void StartEvent(Transform player);
    void EventSuccess();
    void EventFail();
}
