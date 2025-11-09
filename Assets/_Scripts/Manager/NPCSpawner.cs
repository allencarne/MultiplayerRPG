using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] GameObject npc;
    public List<Transform> patrolPointsList;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        GameObject npcInstance = Instantiate(npc, transform.position, Quaternion.identity);
        NetworkObject networkInstance = npcInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        PatrolIdleState patrolIdleState = npcInstance.GetComponentInChildren<PatrolIdleState>();
        patrolIdleState.patrolPoints = patrolPointsList;
    }

    [ClientRpc]
    void PointClientRPC()
    {
        //patrolIdleState.patrolPoints = patrolPointsList;
    }
}
