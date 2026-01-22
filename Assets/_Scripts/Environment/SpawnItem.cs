using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnItem : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField] GameObject ItemPrefab;

    public List<Transform> SpawnPoints;
    [SerializeField] int maxItems;
    int currentItems;
    bool isSpawning = false;

    public override void OnNetworkSpawn()
    {
        InvokeRepeating("CheckIfWeCanSpawn", 0, 10);
    }

    public override void OnNetworkDespawn()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    void CheckIfWeCanSpawn()
    {
        if (!IsServer) return;
        if (isSpawning) return;
        if (currentItems < maxItems) StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(5);

        currentItems++;
        isSpawning = false;
        Spawn();
    }

    void Spawn()
    {
        // Get Random Spawn Position
        int Randomnumber = Random.Range(0, SpawnPoints.Count);
        Vector3 spawnPosition = SpawnPoints[Randomnumber].position;

        // Spawn
        GameObject item = Instantiate(ItemPrefab, spawnPosition, Quaternion.identity);
        NetworkObject networkInstance = item.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        ItemPickup pickup = item.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.Manager = this;
            pickup.SpawnPoint = SpawnPoints[Randomnumber];
        }

        // Remove from list
        SpawnPoints.RemoveAt(Randomnumber);
    }

    public void AddPosition(Transform transform)
    {
        SpawnPoints.Add(transform);
        currentItems--;
    }
}
