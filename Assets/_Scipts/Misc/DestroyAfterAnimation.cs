using Unity.Netcode;
using UnityEngine;

public class DestroyAfterAnimation : NetworkBehaviour
{
    public void AE_DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    public void Net_AE_DestroyAfterAnimation()
    {
        if (IsOwner)
        {
            Debug.Log("HELLO");
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
