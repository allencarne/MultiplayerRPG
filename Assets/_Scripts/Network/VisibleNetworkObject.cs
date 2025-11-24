using Unity.Netcode;
using UnityEngine;

public class VisibleNetworkObject : MonoBehaviour
{
    private void Start()
    {
        VisibilityManager.AddNetworkObject(GetComponent<NetworkObject>());
    }
}
