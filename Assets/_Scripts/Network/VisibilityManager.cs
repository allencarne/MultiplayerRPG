using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class VisibilityManager : NetworkBehaviour
{
    static List<NetworkObject> _networkObjects;
    [SerializeField] float _visibilityRadius;

    public static void AddNetworkObject(NetworkObject networkObject)
    {
        _networkObjects.Add(networkObject);
    }

    private void Start()
    {
        _networkObjects = new List<NetworkObject>();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key == NetworkManager.LocalClientId) continue;

            foreach (NetworkObject networkObject in _networkObjects)
            {
                Transform clientTransform = client.Value.PlayerObject.transform;
                float distance = Vector3.Distance(clientTransform.position, networkObject.transform.position);
                bool isVisible = distance <= _visibilityRadius;

                if (networkObject.IsNetworkVisibleTo(client.Key) != isVisible)
                {
                    if (isVisible)
                    {
                        networkObject.NetworkShow(client.Key);
                    }
                    else
                    {
                        networkObject.NetworkHide(client.Key);
                    }
                }
            }
        }
    }
}
