using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TotemManager : NetworkBehaviour
{
    [Header("Totem")]
    [SerializeField] GameObject totemPrefab;

    [Header("Spawn")]
    public GameObject EnemyPrefab;
    public GameObject BossPrefab;

    [Header("Rewards")]
    public TotemRewards Rewards;

    public List<Transform> SpawnPoints;
    [SerializeField] int maxTotems;
    [HideInInspector] public int currentTotems;
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
        if (currentTotems < maxTotems) StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(5);

        currentTotems++;
        isSpawning = false;
        SpawnTotem();
    }

    void SpawnTotem()
    {
        // Get Random Spawn Position
        int Randomnumber = Random.Range(0, SpawnPoints.Count);
        Vector3 spawnPosition = SpawnPoints[Randomnumber].position;

        // Spawn
        GameObject totem = Instantiate(totemPrefab, spawnPosition, Quaternion.identity);
        NetworkObject networkInstance = totem.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        // Assign Manager
        totem.GetComponent<Totem>().Manager = this;
        totem.GetComponent<Totem>().SpawnPoint = SpawnPoints[Randomnumber];

        // Remove from list
        SpawnPoints.RemoveAt(Randomnumber);
    }

    public void TotemEventCompleted(Transform transform)
    {
        SpawnPoints.Add(transform);
    }
}
