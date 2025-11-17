using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Totem : MonoBehaviour, IInteractable
{
    public Collider2D Trigger;
    public Collider2D Collider2d;
    public SpriteRenderer Sprite;
    public SpriteRenderer Shadow;
    public GameObject ParticleSystem;

    [HideInInspector] public TotemManager Manager;
    [HideInInspector] public Transform SpawnPoint;
    public string DisplayName => "Totem";
    enum EventType { Swarm, Collect, Capture, Dodge }

    public SwarmEvent SwarmEvent;
    public CollectEvent CollectEvent;

    public void Interact(PlayerInteract player)
    {
        //int random = Random.Range(0, 4);
        int random = Random.Range(0, 0);
        switch (random)
        {
            case 0: SwarmEvent.StartEvent(); break;
            //case 1: CollectEvent.StartEvent(); break;
            //case 2: Capture(); break;
            //case 3: Dodge(); break;
        }
    }

    public void ShowTotem(bool isEnabled)
    {
        Trigger.enabled = isEnabled;
        Collider2d.enabled = isEnabled;
        Sprite.enabled = isEnabled;
        Shadow.enabled = isEnabled;
        ParticleSystem.SetActive(isEnabled);
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
