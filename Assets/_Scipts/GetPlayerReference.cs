using Unity.Netcode;
using UnityEngine.Events;

public class GetPlayerReference : NetworkBehaviour
{
    public Player player;
    public UnityEvent OnSpawn;

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton != null)
        {
            TryGetLocalPlayer();
        }
    }

    private void TryGetLocalPlayer()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        foreach (var kvp in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            var networkObject = kvp.Value;

            if (networkObject.OwnerClientId == localClientId)
            {
                Player p = networkObject.GetComponent<Player>();
                if (p != null)
                {
                    player = p;
                    OnSpawn?.Invoke();
                    break;
                }
            }
        }
    }
}
