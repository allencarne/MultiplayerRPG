using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollectEvent : NetworkBehaviour, ITotemEvent
{
    public string EventName => "Collect Event";
    public string EventObjective => $"{(spawnedCollctables.Count - collectableCount)}/{maxCollectable} Collectables";
    public event Action<string> OnObjectiveChanged;

    [Header("References")]
    [SerializeField] Totem totem;
    [SerializeField] TotemParticles particles;
    [SerializeField] GameObject collectablePrefab;

    [Header("Lists")]
    List<GameObject> spawnedCollctables = new();

    int collectableCount;
    int maxCollectable = 10;
    bool isActive = false;

    public void StartEvent(Transform player)
    {
        collectableCount = maxCollectable;
        for (int i = 0; i < maxCollectable; i++) SpawnCollectable();
        OnObjectiveChanged?.Invoke(EventObjective);

        particles.BorderClientRPC();
    }

    public void EventSuccess()
    {
        particles.DisableBorderParcileClientRPC();
    }

    public void EventFail()
    {
        DespawnAll();
    }

    public void SpawnCollectable()
    {
        GameObject collectable = Instantiate(collectablePrefab, totem.GetRandomPoint(9), Quaternion.identity);
        collectable.GetComponent<NetworkObject>().Spawn();

        EventCollectable coll = collectable.GetComponent<EventCollectable>();
        if (coll != null)
        {
            coll.TotemReference = totem;
        }

        spawnedCollctables.Add(collectable);
        isActive = true;
    }

    public void Collected(Player player)
    {
        collectableCount--;
        particles.CollectClientRPC(player.transform.position);
        OnObjectiveChanged?.Invoke(EventObjective);

        if (player != null && !totem.participants.Contains(player))
        {
            totem.participants.Add(player);
        }
    }

    private void Update()
    {
        if (collectableCount == 0 && isActive)
        {
            isActive = false;
            totem.EventSuccess();
        }
    }

    public void DespawnAll()
    {
        if (!IsServer) return;

        foreach (GameObject collectable in spawnedCollctables)
        {
            if (collectable != null)
            {
                particles.DespawnClientRPC(collectable.transform.position);

                NetworkObject networkObject = collectable.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Despawn();
                }
            }
        }

        spawnedCollctables.Clear();
    }
}
