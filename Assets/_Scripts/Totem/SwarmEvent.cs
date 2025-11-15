using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SwarmEvent : MonoBehaviour
{
    //public enum EventState { None, Start, InProgress, Failed, Success }
    //public EventState state;

    [Header("Time")]
    float eventTime = 30;
    float timer;

    [Header("Events")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEnemySpawn;
    public UnityEvent OnEventFailed;
    public UnityEvent OnEventSuccess;

    public void StartEvent()
    {
        timer = eventTime;
        OnEventStart?.Invoke();

        for (int i = 0; i < 3; i++)
        {
            OnEnemySpawn?.Invoke();
        }
    }

    public void FailEvent()
    {
        OnEventFailed?.Invoke();
    }

    private void Update()
    {
        // Decrease the timer every frame
        timer -= Time.deltaTime;

        // Update the countdown text
        //TextEventTime.text = $"Event Ends in {Mathf.CeilToInt(timer)}s";

        // Check if the timer has reached 0 and the event hasn't been completed
        if (timer <= 0f)
        {
            FailEvent();
        }
    }
}
