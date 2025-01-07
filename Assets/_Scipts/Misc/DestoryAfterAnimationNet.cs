using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class DestoryAfterAnimationNet : NetworkBehaviour
{
    public void Net_AE_DestroyAfterAnimation()
    {
        if (IsOwner)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
