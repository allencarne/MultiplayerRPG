using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] GameObject[] npcPrefabs;
    [SerializeField] Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        SpawnNPCs();
    }

    void SpawnNPCs()
    {
        for (int i = 0; i < npcPrefabs.Length; i++)
        {
            GameObject instance = Instantiate(npcPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            NetworkObject spawnedNPC = instance.GetComponent<NetworkObject>();
            spawnedNPC.Spawn();
        }
    }
}