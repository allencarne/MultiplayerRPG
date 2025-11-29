using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class BossEvent : NetworkBehaviour, ITotemEvent
{
    [Header("References")]
    [SerializeField] Totem totem;
    [SerializeField] TotemParticles particles;

    [Header("Lists")]
    List<Enemy> spawnedEnemies = new();

    int enemyCount;
    int maxEnemies = 1;
    bool isActive = false;

    public void StartEvent(Transform player)
    {
        totem.NetEventName.Value = "Boss Event";
        enemyCount = maxEnemies;

        for (int i = 0; i < maxEnemies; i++) SpawnEnemy(player);
        totem.NetEventObjective.Value = $"{(spawnedEnemies.Count - enemyCount)}/{maxEnemies} Enemies";

        particles.BorderClientRPC();
    }

    public void EventSuccess()
    {
        particles.DisableBorderParcileClientRPC();
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
        GameObject enemyInstance = Instantiate(totem.Manager.BossPrefab, totem.GetRandomPoint(6), Quaternion.identity);
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
        totem.NetEventObjective.Value = $"{(spawnedEnemies.Count - enemyCount)}/{maxEnemies} Enemies";

        if (player != null && !totem.participants.Contains(player))
        {
            totem.participants.Add(player);
        }
    }

    public void DespawnAllEnemies()
    {
        if (!IsServer) return;

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
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
