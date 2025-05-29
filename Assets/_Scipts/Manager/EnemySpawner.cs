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

        int spawnDelay = Random.Range(1, 6);

        yield return new WaitForSeconds(spawnDelay);

        canSpawn = true;
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        // Get Local Position
        Vector2 localOffset = new Vector2(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2)
        );

        // Handle Rotation and Position
        Vector3 worldOffset = transform.rotation * (Vector3)localOffset;
        Vector3 spawnPosition = transform.position + worldOffset;

        if (CheckForObstacles(spawnPosition))
        {
            SpawnEnemy();
            return;
        }

        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
        NetworkObject networkInstance = enemyInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        enemyInstance.GetComponent<Enemy>().EnemySpawnerReference = this;

        currentEnemyCount++;
    }

    bool CheckForObstacles(Vector3 spawnPosition)
    {
        Collider2D hit = Physics2D.OverlapCircle(spawnPosition, 1f);
        if (hit != null && hit.CompareTag("Obstacle"))
        {
            return true;
        }
        return false;
    }

    public void DecreaseEnemyCount()
    {
        currentEnemyCount--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        // Apply rotation to the box using a matrix
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(Vector3.zero, size); // Vector3.zero because position is already applied via matrix
    }
}
