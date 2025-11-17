using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SwarmEvent : MonoBehaviour
{
    public enum EventState { None, InProgress, Failed, Success }
    public EventState state;

    [Header("Lists")]
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    [SerializeField] Item[] rewards;

    [Header("Time")]
    public int EnemyCount;
    float eventTime = 30;
    float timer;

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

        Reward();
    }

    void Reward()
    {
        int amountofItems = Random.Range(1, 4);

        for (int i = 0; i < amountofItems; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 1;
            int randomItem = Random.Range(0, rewards.Length);

            GameObject itemInstance = Instantiate(rewards[randomItem].Prefab, randomPos, Quaternion.identity);
            NetworkObject networkInstance = itemInstance.GetComponent<NetworkObject>();
            networkInstance.Spawn();
        }
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

        GameObject enemyInstance = Instantiate(totem.Manager.EnemyPrefab, randomPos, Quaternion.identity);
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
