using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SwarmEvent : MonoBehaviour
{
    public enum EventState { None, InProgress, Failed, Success }
    public EventState state;

    [Header("Time")]
    float eventTime = 30;
    float timer;
    public int EnemyCount;

    [SerializeField] TextMeshProUGUI eventText;

    [Header("Events")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEnemySpawn;
    public UnityEvent OnEventFailed;
    public UnityEvent OnEventSuccess;

    public void StartEvent()
    {
        timer = eventTime;
        OnEventStart?.Invoke();
        state = EventState.InProgress;
        eventText.text = "Start!";

        for (int i = 0; i < 3; i++)
        {
            EnemyCount++;
            OnEnemySpawn?.Invoke();
        }
    }

    private void Update()
    {
        if (state != EventState.InProgress) return;

        // Decrease the timer every frame
        timer -= Time.deltaTime;

        // Update the countdown text
        eventText.text = $"Event Ends in {Mathf.CeilToInt(timer)}s";

        // Success
        if (EnemyCount == 0)
        {
            SuccessEvent();
        }

        // Fail
        if (timer <= 0f)
        {
            FailEvent();
        }
    }

    void FailEvent()
    {
        eventText.text = "Failed!";
        state = EventState.Failed;
        OnEventFailed?.Invoke();
    }

    void SuccessEvent()
    {
        eventText.text = "Success!";
        state = EventState.Success;
        OnEventFailed?.Invoke();
    }
}
