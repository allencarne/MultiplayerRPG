using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GetPlayerReference : NetworkBehaviour
{
    public Player player;
    public UnityEvent OnSpawn;

    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        while (NetworkManager.Singleton.LocalClient == null ||
               NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            yield return null; // wait until player is spawned
        }

        player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
    }
}