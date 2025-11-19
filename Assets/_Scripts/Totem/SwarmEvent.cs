using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class SwarmEvent : NetworkBehaviour
{
    public enum EventState { None, InProgress, Failed, Success }
    public EventState state;

    [Header("References")]
    [SerializeField] TotemParticles particles;

    [Header("Lists")]
    public List<Enemy> spawnedEnemies = new List<Enemy>();
    [SerializeField] Item coin;
    [SerializeField] Item[] rewards;

    [Header("Coin")]
    public int Coins;
    public int MaxCoinReward;

    [Header("Time")]
    int enemyCount;
    float eventTime = 30;
    float timer;

    [SerializeField] TextMeshProUGUI eventText;

    [Header("Events")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEventFailed;
    public UnityEvent OnEventSuccess;

    public void StartEvent(Transform player)
    {
        timer = eventTime;
        OnEventStart?.Invoke();
        state = EventState.InProgress;
        UpdateUIClientRPC("Start!");
        InvokeRepeating("UpdateEvent", 0,1);

        for (int i = 0; i < 3; i++)
        {
            enemyCount++;
            SpawnEnemy(player);
        }
    }

    [ClientRpc]
    void UpdateUIClientRPC(string text)
    {
        eventText.text = text;
    }

    void UpdateEvent()
    {
        if (state != EventState.InProgress) return;

        // Decrease the timer
        timer -= 1f;

        // Update the countdown text
        UpdateUIClientRPC($"Event Ends in {Mathf.CeilToInt(timer)}s");

        // Success
        if (enemyCount == 0) SuccessEvent();

        // Fail
        if (timer <= 0f) FailEvent();
    }

    void SuccessEvent()
    {
        CancelInvoke();
        UpdateUIClientRPC("Success!");
        state = EventState.Success;
        OnEventSuccess?.Invoke();

        CoinReward();
        ItemReward();
    }

    void FailEvent()
    {
        CancelInvoke();
        UpdateUIClientRPC("Failed!");
        state = EventState.Failed;
        DespawnAllEnemies();
        OnEventFailed?.Invoke();
    }

    public void SpawnEnemy(Transform player)
    {
        Totem totem = GetComponent<Totem>();
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 6;

        GameObject enemyInstance = Instantiate(totem.Manager.EnemyPrefab, randomPos, Quaternion.identity);
        enemyInstance.GetComponent<NetworkObject>().Spawn();

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        EnemyStateMachine stateMachine = enemyInstance.GetComponent<EnemyStateMachine>();

        enemy.TotemReference = totem;
        spawnedEnemies.Add(enemy);

        // Assign Enemy Target
        stateMachine.Target = player;
        stateMachine.IsPlayerInRange = true;
    }

    public void EnemyDeath()
    {
        enemyCount--;
    }

    public void DespawnAllEnemies()
    {
        if (!IsServer) return;

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                enemy.IsDead = true;

                particles.DespawnClientRPC(enemy.transform.position);

                NetworkObject networkObject = enemy.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Despawn();
                }
            }
        }

        spawnedEnemies.Clear();
    }

    void CoinReward()
    {
        int amountOfItems = Random.Range(1, Coins + 1);

        for (int i = 0; i < amountOfItems; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 1f;

            GameObject instance = Instantiate(coin.Prefab, randomPos, Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn();

            ItemPickup pickup = instance.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.Quantity = Random.Range(0, MaxCoinReward);
            }
        }
    }

    void ItemReward()
    {
        foreach (Item reward in rewards)
        {
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 1f;

                GameObject instance = Instantiate(reward.Prefab, randomPos, Quaternion.identity);
                instance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
