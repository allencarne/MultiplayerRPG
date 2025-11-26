using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Totem : NetworkBehaviour, IInteractable
{
    public string DisplayName => "Totem";
    public ITotemEvent CurrentEvent { get; private set; }
    public LayerMask obstacleLayerMask;

    [Header("References")]
    [HideInInspector] public TotemManager Manager;
    [HideInInspector] public Transform SpawnPoint;
    [SerializeField] TextMeshProUGUI eventText;
    [SerializeField] Collider2D totemObstacleCollider;
    public SwarmEvent SwarmEvent;
    public CollectEvent CollectEvent;
    public BossEvent BossEvent;

    [Header("Events")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEventSuccess;
    public UnityEvent OnEventFail;

    public List<Player> participants = new();

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
                case 0: CurrentEvent = SwarmEvent; SwarmEvent.StartEvent(player); break;
                case 1: CurrentEvent = CollectEvent; CollectEvent.StartEvent(player); break;
                case 2: CurrentEvent = BossEvent; BossEvent.StartEvent(player); break;
            }
        }

        EventStart();
    }

    public void EventStart()
    {
        timer = eventTime;
        InvokeRepeating("UpdateEvent", 0, 1);
        UpdateUIClientRPC("Start!");
        OnEventStart?.Invoke();
    }

    void UpdateEvent()
    {
        // Decrease the timer
        timer -= 1f;

        // Update the countdown text
        UpdateUIClientRPC($"Event Ends in {Mathf.CeilToInt(timer)}s");

        // Fail
        if (timer <= 0f) EventFail();
    }

    [ClientRpc]
    void UpdateUIClientRPC(string text)
    {
        eventText.text = text;
    }

    public void EventSuccess()
    {
        CancelInvoke();
        UpdateUIClientRPC("Success!");
        OnEventSuccess?.Invoke();
        CurrentEvent?.EventSuccess();

        foreach (Player player in participants)
        {
            Manager.Rewards.ExperienceRewards(player);

            ulong targetClientId = player.OwnerClientId;

            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { targetClientId }
                }
            };

            Manager.Rewards.QuestParticipationClientRPC(CurrentEvent.EventName, rpcParams);
        }
    }

    public void EventFail()
    {
        CancelInvoke();
        UpdateUIClientRPC("Failed!");
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
}