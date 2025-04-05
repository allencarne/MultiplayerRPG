using Unity.Netcode;

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
