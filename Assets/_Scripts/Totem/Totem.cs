using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Totem : NetworkBehaviour, IInteractable
{
    public string DisplayName => "Totem";
    enum EventType { Swarm, Collect, Capture, Dodge }
    public ITotemEvent CurrentEvent { get; private set; }

    [Header("Visuals")]
    [SerializeField] Collider2D Trigger;
    [SerializeField] Collider2D Collider2d;
    [SerializeField] SpriteRenderer Sprite;
    [SerializeField] SpriteRenderer Shadow;
    [SerializeField] GameObject ParticleSystem;
    [SerializeField] Collider2D eventTrigger;

    [Header("References")]
    [HideInInspector] public TotemManager Manager;
    [HideInInspector] public Transform SpawnPoint;

    [Header("Events")]
    public SwarmEvent SwarmEvent;
    public CollectEvent CollectEvent;


    public void Interact(PlayerInteract player)
    {
        StartEventServerRpc(player.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    void StartEventServerRpc(NetworkObjectReference playerRef)
    {
        if (!playerRef.TryGet(out NetworkObject networkObject)) return;

        Transform player = networkObject.GetComponent<Transform>();
        if (player != null)
        {
            int random = Random.Range(0, 0);

            switch (random)
            {
                case 0: SwarmEvent.StartEvent(player); CurrentEvent = SwarmEvent; break;
                    //case 1: CollectEvent.StartEvent(player); break;
            }
        }
    }

    [ClientRpc]
    public void ShowTotemClientRPC()
    {
        eventTrigger.enabled = true;

        Trigger.enabled = false;
        Collider2d.enabled = false;
        Sprite.enabled = false;
        Shadow.enabled = false;
        ParticleSystem.SetActive(false);
    }


    public void DespawnTotem()
    {
        StartCoroutine(DespawnDelay());
    }

    IEnumerator DespawnDelay()
    {
        yield return new WaitForSeconds(3);
        Manager.currentTotems--;
        Manager.TotemEventCompleted(SpawnPoint);
        GetComponent<NetworkObject>().Despawn();
    }
}
