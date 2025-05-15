using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DespawnDelay : NetworkBehaviour
{
    public IEnumerator DespawnAfterDuration(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}
