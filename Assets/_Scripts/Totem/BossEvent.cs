using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class BossEvent : NetworkBehaviour, ITotemEvent
{
    [Header("Enemy")]
    public GameObject EnemyPrefab;

    [Header("References")]
    [SerializeField] Totem totem;
    [SerializeField] TotemParticles particles;
    [SerializeField] TotemRewards rewards;

    [Header("Lists")]
    List<Enemy> spawnedEnemies = new();
    List<Player> participants = new();

    int enemyCount;
    int maxEnemies = 1;

    bool isActive = false;

    public string EventName => "Boss Event";
    public string EventObjective => $"{(spawnedEnemies.Count - enemyCount)}/{spawnedEnemies.Count} Enemies";

    public event Action<string> OnObjectiveChanged;

    public void StartEvent(Transform player)
    {
        enemyCount = maxEnemies;
        for (int i = 0; i < maxEnemies; i++) SpawnEnemy(player);
        OnObjectiveChanged?.Invoke(EventObjective);

        particles.BorderClientRPC();
    }

    public void EventSuccess()
    {
        particles.DisableBorderParcileClientRPC();

        foreach (Player player in participants)
        {
            rewards.ExperienceRewards(player);
        }
    }

    public void EventFail()
    {
        DespawnAllEnemies();
    }

    private void Update()
    {
        if (enemyCount == 0 && isActive)
        {
            isActive = false;
            totem.EventSuccess();
        }
    }

    public void SpawnEnemy(Transform player)
    {
        Vector2 randomPos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 6;

        GameObject enemyInstance = Instantiate(EnemyPrefab, randomPos, Quaternion.identity);
        enemyInstance.GetComponent<NetworkObject>().Spawn();

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        EnemyStateMachine stateMachine = enemyInstance.GetComponent<EnemyStateMachine>();

        enemy.TotemReference = totem;
        spawnedEnemies.Add(enemy);

        // Assign Enemy Target
        stateMachine.Target = player;
        stateMachine.IsPlayerInRange = true;

        isActive = true;
    }

    public void EnemyDeath(Player player)
    {
        enemyCount--;
        OnObjectiveChanged?.Invoke(EventObjective);

        if (player != null && !participants.Contains(player))
        {
            participants.Add(player);
        }
    }

    public void DespawnAllEnemies()
    {
        if (!IsServer) return;

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                enemy.IsDead = true; // Should Stop Telegraphs when enemy dies, but currently does not work

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
}
