using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Totem : MonoBehaviour, IInteractable
{
    public List<Enemy> spawnedEnemies = new List<Enemy>();

    public Collider2D Trigger;
    public Collider2D Collider2d;
    public SpriteRenderer Sprite;
    public SpriteRenderer Shadow;

    [HideInInspector] public TotemManager Manager;
    [HideInInspector] public Transform SpawnPoint;
    public string DisplayName => "Totem";
    enum EventType { Swarm, Collect, Capture, Dodge }

    SwarmEvent SwarmEvent;
    CollectEvent CollectEvent;

    private void Awake()
    {
        SwarmEvent = GetComponent<SwarmEvent>();
        CollectEvent = GetComponent<CollectEvent>();
    }

    public void Interact(PlayerInteract player)
    {
        //int random = Random.Range(0, 4);
        int random = Random.Range(0, 0);
        switch (random)
        {
            case 0: SwarmEvent.StartEvent(); break;
            //case 1: CollectEvent.StartEvent(); break;
            //case 2: Capture(); break;
            //case 3: Dodge(); break;
        }
    }

    public void ShowTotem(bool isEnabled)
    {
        Trigger.enabled = isEnabled;
        Collider2d.enabled = isEnabled;
        Sprite.enabled = isEnabled;
        Shadow.enabled = isEnabled;
    }

    public void SpawnEnemy()
    {
        Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 6;

        GameObject enemyInstance = Instantiate(Manager.EnemyPrefab, randomPos, Quaternion.identity, transform);
        NetworkObject networkInstance = enemyInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        Enemy enemy = enemyInstance.GetComponent<Enemy>();
        enemy.TotemReference = this;
        spawnedEnemies.Add(enemy);
    }

    public void EnemyDeath()
    {
        SwarmEvent.EnemyCount--;
    }

    public void DespawnTotem()
    {
        StartCoroutine(DespawnDelay());
    }

    IEnumerator DespawnDelay()
    {
        yield return new WaitForSeconds(3);
        Manager.currentTotems--;
        Manager.TotemEventCompleted(SpawnPoint);
        GetComponent<NetworkObject>().Despawn();
    }

    public void DespawnAllEnemies()
    {
        if (!Manager.IsServer) return;

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
