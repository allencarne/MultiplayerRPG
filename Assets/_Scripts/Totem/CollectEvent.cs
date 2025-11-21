using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollectEvent : NetworkBehaviour, ITotemEvent
{
    [SerializeField] GameObject collectablePrefab;
    List<GameObject> spawnedCollctables = new();
    int collectableCount;
    int maxCollectable = 10;

    public string EventName => "Collect Event";
    public string EventObjective => $"{(spawnedCollctables.Count - collectableCount)}/{maxCollectable} Collectables";

    public event Action<string> OnObjectiveChanged;

    [SerializeField] TotemParticles particles;
    [SerializeField] Totem totem;
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
        Vector2 randomPos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 9;

        GameObject collectable = Instantiate(collectablePrefab, randomPos, Quaternion.identity);
        collectable.GetComponent<NetworkObject>().Spawn();

        EventCollectable coll = collectable.GetComponent<EventCollectable>();
        if (coll != null)
        {
            coll.TotemReference = totem;
        }

        spawnedCollctables.Add(collectable);
        isActive = true;
    }

    public void Collected(Vector2 position)
    {
        collectableCount--;
        particles.CollectClientRPC(position);
        OnObjectiveChanged?.Invoke(EventObjective);
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
