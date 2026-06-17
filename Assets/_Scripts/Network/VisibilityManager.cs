using Unity.Netcode;
using UnityEngine;

public class VisibilityManager : NetworkBehaviour
{
    public float visibilityRange = 50f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // The server handles visibility checks and should subscribe when spawned locally on the server-side.
            NetworkObject.CheckObjectVisibility += CheckVisibility;
            // If we want to continually update, we don't need to check every frame but should check at least once per tick
            NetworkManager.NetworkTickSystem.Tick += UpdateVisibility;
        }
        base.OnNetworkSpawn();
    }

    // Returns true if the client is close enough to see this object
    // This is automatically invoked when spawning the network prefab
    private bool CheckVisibility(ulong clientId)
    {
        if (!IsSpawned) return false;

        // We can do a simple distance check between the NetworkObject instance position and the client
        NetworkObject netObject = NetworkManager.ConnectedClients[clientId].PlayerObject;
        return Vector3.Distance(netObject.transform.position, transform.position) <= visibilityRange;
    }

    // Ensure that clients who move in and out of range have their visibility updated
    // Called every network tick
    private void UpdateVisibility()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            bool shouldBeVisible = CheckVisibility(clientId);
            bool isVisible = NetworkObject.IsNetworkVisibleTo(clientId);

            //If the visibility state has changed, update it for the client
            if (shouldBeVisible != isVisible)
            {
                if (shouldBeVisible)
                {
                    NetworkObject.NetworkShow(clientId);
                }
                else
                {
                    NetworkObject.NetworkHide(clientId);
                }
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
