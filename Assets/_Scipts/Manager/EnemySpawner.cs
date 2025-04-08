using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Enemy")]
    [SerializeField] GameObject enemyPrefab;

    [Header("Box Size")]
    [SerializeField] Vector2 size;

    [Header("Enemy Count")]
    [SerializeField] int maxEnemyCount;

    [Header("Color")]
    [SerializeField] Color color;

    int currentEnemyCount;
    float spawnDelay = 6;
    bool canSpawn = true;

    private void Update()
    {
        if (!IsServer) return;

        if (currentEnemyCount < maxEnemyCount)
        {
            if (canSpawn)
            {
                StartCoroutine(Delay());
            }
        }
    }

    IEnumerator Delay()
    {
        canSpawn = false;

        yield return new WaitForSeconds(spawnDelay);

        canSpawn = true;
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2), 0f, 0f);
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);

        NetworkObject networkInstance = enemyInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        enemyInstance.GetComponent<Enemy>().EnemySpawnerReference = this;

        currentEnemyCount++;
    }

    public void DecreaseEnemyCount()
    {
        currentEnemyCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
