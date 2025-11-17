using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] Item coin;
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

        Coins();
        Reward();
    }

    void Coins()
    {
        int amountOfItems = Random.Range(1, 4);   // Spawn 1–3 items

        for (int i = 0; i < amountOfItems; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 1f;

            GameObject instance = Instantiate(coin.Prefab, randomPos, Quaternion.identity);
            NetworkObject netObj = instance.GetComponent<NetworkObject>();
            netObj.Spawn();
        }
    }

    void Reward()
    {
        int totalWeight = 0;

        foreach (Item reward in rewards)
        {
            totalWeight += reward.DropChance;
        }

        int roll = Random.Range(0, totalWeight);
        Item selected = null;

        foreach (Item r in rewards)
        {
            if (roll < r.DropChance)
            {
                selected = r;
                break;
            }

            roll -= r.DropChance;
        }

        if (selected == null) return;

        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 1f;

        GameObject instance = Instantiate(selected.Prefab, randomPos, Quaternion.identity);
        NetworkObject netObj = instance.GetComponent<NetworkObject>();
        netObj.Spawn();
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
