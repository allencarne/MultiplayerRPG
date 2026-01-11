using Unity.Netcode;
using UnityEngine;

public class PrayStatue : NetworkBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Material startMat;
    [SerializeField] Material endMat;

    [SerializeField] string area;
    [SerializeField] int index;
    bool canClaim = true;

    public string DisplayName => "Praying Statue";

    private void Start()
    {
        string status = PlayerPrefs.GetString($"{area}_Statue_{index}", "Incomplete");

        if (status == "Completed")
        {
            canClaim = false;
            sprite.material = endMat;
        }
    }

    public void Interact(PlayerInteract player)
    {
        if (!canClaim) return;
        StartEventServerRpc(player.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    void StartEventServerRpc(NetworkObjectReference playerRef)
    {
        if (!playerRef.TryGet(out NetworkObject networkObject)) return;
        PlayerStats stats = networkObject.GetComponent<PlayerStats>();

        if (stats != null)
        {
            canClaim = false;
            PlayerPrefs.SetString($"{area}_Statue_{index}", "Completed");
            PlayerPrefs.Save();

            stats.IncreaseAttribuePoints();
            sprite.material = endMat;
        }
    }
}
