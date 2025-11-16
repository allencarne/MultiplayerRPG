using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SwarmEvent : MonoBehaviour
{
    public List<Enemy> spawnedEnemies = new List<Enemy>();

    public enum EventState { None, InProgress, Failed, Success }
    public EventState state;

    [Header("Time")]
    float eventTime = 30;
    float timer;
    public int EnemyCount;

    [SerializeField] TextMeshProUGUI eventText;

    [Header("Events")]
    public UnityEvent OnEventStart;
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
            SpawnEnemy();
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

    void SuccessEvent()
    {
        eventText.text = "Success!";
        state = EventState.Success;
        OnEventSuccess?.Invoke();
    }

    void FailEvent()
    {
        eventText.text = "Failed!";
        state = EventState.Failed;
        DespawnAllEnemies();
        OnEventFailed?.Invoke();
    }

    public void EnemyDeath()
    {
        EnemyCount--;
    }

    public void SpawnEnemy()
    {
        Totem totem = GetComponent<Totem>();
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 6;

        GameObject enemyInstance = Instantiate(totem.Manager.EnemyPrefab, randomPos, Quaternion.identity, transform);
        NetworkObject networkInstance = enemyInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        enemy.TotemReference = totem;
        spawnedEnemies.Add(enemy);
    }

    public void DespawnAllEnemies()
    {
        Totem totem = GetComponent<Totem>();
        if (!totem.Manager.IsServer) return;

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Despawn();
                }
            }
        }

        spawnedEnemies.Clear();
    }
}
