using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Totem : NetworkBehaviour, IInteractable
{
    public NetworkVariable<FixedString64Bytes> NetEventName =
    new(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString64Bytes> NetEventObjective =
        new(writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<float> NetEventTime =
        new(writePerm: NetworkVariableWritePermission.Server);

    public string DisplayName => "Totem";
    public ITotemEvent CurrentEvent { get; private set; }
    public LayerMask obstacleLayerMask;

    [Header("Participants")]
    public List<Player> participants = new();

    [Header("References")]
    [HideInInspector] public TotemManager Manager;
    [HideInInspector] public Transform SpawnPoint;
    [SerializeField] Collider2D totemObstacleCollider;

    [Header("Types")]
    public SwarmEvent SwarmEvent;
    public CollectEvent CollectEvent;
    public BossEvent BossEvent;

    [Header("Events")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEventSuccess;
    public UnityEvent OnEventFail;

    [Header("Time")]
    float eventTime = 30;
    float timer;

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
            int random = Random.Range(0, 3);

            switch (random)
            {
                case 0: 
                    CurrentEvent = SwarmEvent; 
                    SwarmEvent.StartEvent(player);
                    break;
                case 1: 
                    CurrentEvent = CollectEvent; 
                    CollectEvent.StartEvent(player); 
                    break;
                case 2: 
                    CurrentEvent = BossEvent; 
                    BossEvent.StartEvent(player); 
                    break;
            }

            SyncEventClientRpc(random);
        }

        NetEventTime.Value = eventTime;
        EventStart();
    }

    public void EventStart()
    {
        timer = eventTime;
        InvokeRepeating("UpdateEvent", 0, 1);
        OnEventStart?.Invoke();
    }

    void UpdateEvent()
    {
        timer -= 1f;
        NetEventTime.Value = timer;

        if (timer <= 0f) EventFail();
    }

    public void EventSuccess()
    {
        CancelInvoke();
        OnEventSuccess?.Invoke();
        CurrentEvent?.EventSuccess();

        foreach (Player player in participants)
        {
            Manager.Rewards.ExperienceRewards(player);
            Manager.Rewards.CoinRewards(player);
            Manager.Rewards.ItemRewards(player);

            ulong targetClientId = player.OwnerClientId;

            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { targetClientId }
                }
            };

            Manager.Rewards.QuestParticipationClientRPC(NetEventName.Value.ToString(), rpcParams);
        }
    }

    public void EventFail()
    {
        CancelInvoke();
        OnEventFail?.Invoke();
        CurrentEvent?.EventFail();
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

    public Vector2 GetRandomPoint(float radius)
    {
        totemObstacleCollider.enabled = false;
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * radius;
            Vector2 randomDir = randomPos - (Vector2)transform.position;
            float distance = randomDir.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDir.normalized, distance, obstacleLayerMask);

            if (hit.collider != null)
            {
                randomPos = hit.point;
            }
            else
            {
                return randomPos;
            }

            Debug.DrawLine(transform.position, randomPos, Color.red, 1f);
        }

        return transform.position;
    }

    [ClientRpc]
    private void SyncEventClientRpc(int eventType)
    {
        switch (eventType)
        {
            case 0: CurrentEvent = SwarmEvent; break;
            case 1: CurrentEvent = CollectEvent; break;
            case 2: CurrentEvent = BossEvent; break;
        }
    }
}