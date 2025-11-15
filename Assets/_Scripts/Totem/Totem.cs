using Unity.Netcode;
using UnityEngine;

public class Totem : MonoBehaviour, IInteractable
{
    public Collider2D Trigger;
    public Collider2D Collider2d;
    public SpriteRenderer Sprite;
    public SpriteRenderer Shadow;

    [HideInInspector] public TotemManager Manager;
    public string DisplayName => "Totem";
    enum EventType { Swarm, Collect, Capture, Dodge }

    SwarmEvent SwarmEvent;
    CollectEvent CollectEvent;

    private void Awake()
    {
        SwarmEvent = GetComponent<SwarmEvent>();
        CollectEvent = GetComponent<CollectEvent>();
    }

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
    }

    public void SpawnEnemy()
    {
        GameObject enemyInstance = Instantiate(Manager.EnemyPrefab, transform.position, Quaternion.identity, transform);
        NetworkObject networkInstance = enemyInstance.GetComponent<NetworkObject>();
        networkInstance.Spawn();

        enemyInstance.GetComponent<Enemy>().TotemReference = this;
    }

    public void EnemyDeath()
    {
        Debug.Log("EnemyDeath");
    }
}
