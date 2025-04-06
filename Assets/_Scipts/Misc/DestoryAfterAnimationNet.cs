using Unity.Netcode;

public class DestoryAfterAnimationNet : NetworkBehaviour
{
    public void Net_AE_DestroyAfterAnimation()
    {
        if (IsServer)
        {
            NetworkObject.Destroy(gameObject);
            //GetComponent<NetworkObject>().Despawn();
        }
    }
}
