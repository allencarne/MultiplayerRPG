using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TotemManager : NetworkBehaviour
{
    [Header("Totem")]
    [SerializeField] GameObject totemPrefab;

    [Header("Enemy")]
    public GameObject EnemyPrefab;

    public List<Transform> SpawnPoints;
    int maxTotems = 3;
    int currentTotems;
    bool isSpawning = false;

    private void Update()
    {
        if (!IsServer) return;
        if (currentTotems < maxTotems && !isSpawning) StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(5);
        currentTotems++;
        isSpawning = false;

        // Get Random Spawn Position
        int Randomnumber = Random.Range(0, SpawnPoints.Count);
        Vector3 spawnPosition = SpawnPoints[Randomnumber].position;

        // Remove from list
        SpawnPoints.RemoveAt(Randomnumber);

        // Spawn
        GameObject totem = Instantiate(totemPrefab, spawnPosition, Quaternion.identity, transform);
        NetworkObject networkInstance = totem.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        // Assign Manager
        totem.GetComponent<Totem>().Manager = this;
    }

    public void TotemEventCompleted(int index)
    {
        SpawnPoints.Add(SpawnPoints[index]);
    }
}
