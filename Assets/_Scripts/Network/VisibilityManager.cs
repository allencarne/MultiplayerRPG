using Unity.Netcode;
using UnityEngine;

public class VisibilityManager : NetworkBehaviour
{
    public float visibilityRange = 50f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility += CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick += UpdateVisibility;
        }
        base.OnNetworkSpawn();
    }

    private bool CheckVisibility(ulong clientId)
    {
        if (!IsSpawned) return false;

        var playerObject = NetworkManager.ConnectedClients[clientId].PlayerObject;
        return Vector3.Distance(playerObject.transform.position, transform.position) <= visibilityRange;
    }

    private void UpdateVisibility()
    {
        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            var shouldBeVisible = CheckVisibility(clientId);
            var isVisible = NetworkObject.IsNetworkVisibleTo(clientId);

            if (shouldBeVisible != isVisible)
            {
                if (shouldBeVisible)
                    NetworkObject.NetworkShow(clientId);
                else
                    NetworkObject.NetworkHide(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkObject.CheckObjectVisibility -= CheckVisibility;
            NetworkManager.NetworkTickSystem.Tick -= UpdateVisibility;
        }
        base.OnNetworkDespawn();
    }
}
