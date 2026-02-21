using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [Header("Variables")]
    public string Enemy_ID;
    public string Enemy_Name;
    [SerializeField] float startingHealth;
    public int Enemy_Level;
    public float expToGive;
    public float TotalPatience;

    [Header("Components")]
    [HideInInspector] public EnemySpawner EnemySpawnerReference;
    [HideInInspector] public Totem TotemReference;
    [SerializeField] GameObject expPrefab;
    [SerializeField] EnemyStateMachine stateMachine;
    public CharacterStats stats;

    [Header("Sprites")]
    public SpriteRenderer bodySprite;
    public SpriteRenderer shadowSprite;

    [Header("UI")]
    [SerializeField] HealthBar healthBar;
    public PatienceBar PatienceBar;
    public CastBar CastBar;

    [Header("Bools")]
    public bool IsDummy;
    public bool IsRegen;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            stats.net_TotalHP.Value = startingHealth;
            stats.net_BaseHP.Value = startingHealth;
            stats.net_CurrentHP.Value = startingHealth;
        }

        stats.OnEnemyDamaged.AddListener(Damaged);
        stats.OnEnemyDeath.AddListener(Death);

        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);

        stats.net_CurrentHP.OnValueChanged += OnHPChanged;
        stats.net_TotalHP.OnValueChanged += OnMaxHPChanged;
    }

    public override void OnNetworkDespawn()
    {
        stats.OnEnemyDamaged.RemoveListener(Damaged);
        stats.OnEnemyDeath.RemoveListener(Death);

        stats.OnDamaged.RemoveListener(TakeDamage);
        stats.OnDamageDealt.RemoveListener(DealDamage);

        stats.net_CurrentHP.OnValueChanged -= OnHPChanged;
        stats.net_TotalHP.OnValueChanged -= OnMaxHPChanged;
    }

    void OnHPChanged(float previousValue, float newValue)
    {
        CheckStopRegen();
    }

    void OnMaxHPChanged(float previousValue, float newValue)
    {
        CheckStopRegen();
    }

    void CheckStopRegen()
    {
        if (IsRegen && stats.net_CurrentHP.Value >= stats.net_TotalHP.Value)
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }

    void TakeDamage(float damage)
    {
        if (stateMachine.isResetting) return;

        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    void DealDamage()
    {
        if (stateMachine.isResetting) return;

        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    void Damaged(NetworkObject attackerID)
    {
        if (!IsServer) return;
        if (IsDummy) PatienceBar.Patience.Value = 0;

        ulong targetClientId = attackerID.OwnerClientId;
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { targetClientId }
            }
        };

        PlayerQuest quest = attackerID.GetComponent<PlayerQuest>();
        if (quest != null) UpdateObjectiveClientRpc(ObjectiveType.Hit, Enemy_ID, 1, attackerID.NetworkObjectId, rpcParams);

        TargetAttacker(attackerID);
        EventParticipate(attackerID);
    }

    void Death(NetworkObject attackerID)
    {
        if (!IsServer) return;
        if (IsDummy) return;

        Transform attackerPosition = attackerID.GetComponent<Transform>();
        if (attackerPosition != null) StartCoroutine(DropEXP(attackerPosition));

        PlayerExperience exp = attackerID.gameObject.GetComponent<PlayerExperience>();
        if (exp) exp.IncreaseEXP(expToGive);

        ulong targetClientId = attackerID.OwnerClientId;
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { targetClientId }
            }
        };

        PlayerQuest quest = attackerID.GetComponent<PlayerQuest>();
        if (quest != null) UpdateObjectiveClientRpc(ObjectiveType.Kill, Enemy_ID, 1, attackerID.NetworkObjectId, rpcParams);

        if (stateMachine.hasMightOnStart || stateMachine.hasSwiftnessOnStart || stateMachine.hasAlacrityOnStart || stateMachine.hasProtectionOnStart)
        {
            StartBuffsClientRPC(
                attackerID.NetworkObjectId,
                stateMachine.hasMightOnStart,
                stateMachine.hasSwiftnessOnStart,
                stateMachine.hasAlacrityOnStart,
                stateMachine.hasProtectionOnStart,
                rpcParams
            );
        }

        EventDeath(attackerID);
        NPCEventParticipation(attackerID);

        DeathClientRpc();
        stateMachine.SetState(EnemyStateMachine.State.Death);
    }

    void EventDeath(NetworkObject attackerID)
    {
        Player player = attackerID.GetComponent<Player>();
        if (player != null && TotemReference != null)
        {
            switch (TotemReference.CurrentEvent)
            {
                case SwarmEvent swarm: swarm.EnemyDeath(); break;
                case BossEvent boss: boss.EnemyDeath(); break;
            }
        }
    }

    void EventParticipate(NetworkObject attackerID)
    {
        Player player = attackerID.GetComponent<Player>();
        if (player != null && TotemReference != null)
        {
            switch (TotemReference.CurrentEvent)
            {
                case SwarmEvent swarm: swarm.Participate(player); break;
                case BossEvent boss: boss.Participate(player); break;
            }
        }
    }

    void NPCEventParticipation(NetworkObject attackerID)
    {
        NPCStateMachine npc = attackerID.GetComponent<NPCStateMachine>();
        if (npc != null) npc.Target = null;

        if (npc != null && TotemReference != null)
        {
            switch (TotemReference.CurrentEvent)
            {
                case SwarmEvent swarm: swarm.DeathByNPC(); break;
                case BossEvent boss: boss.DeathByNPC(); break;
            }
        }
    }

    void TargetAttacker(NetworkObject attackerID)
    {
        if (stateMachine.state == EnemyStateMachine.State.Reset) return;

        if (stateMachine.Target == null)
        {
            stateMachine.Target = attackerID.transform;
            stateMachine.IsPlayerInRange = true;
        }
        else if (stateMachine.SecondTarget == null && stateMachine.Target != attackerID.transform)
        {
            stateMachine.SecondTarget = attackerID.transform;
            stateMachine.IsPlayerInRange = true;
        }
    }

    IEnumerator DropEXP(Transform attackerPosition)
    {
        for (int i = 0; i < expToGive; i++)
        {
            GameObject expObj = Instantiate(expPrefab, transform.position, transform.rotation);
            expObj.GetComponent<TravelToTarget>().target = attackerPosition;
            yield return new WaitForSeconds(.1f);
        }
    }

    [ClientRpc]
    void UpdateObjectiveClientRpc(ObjectiveType type, string id, int amount, ulong attackerNetworkObjectId, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(attackerNetworkObjectId, out NetworkObject netObj))
        {
            PlayerQuest quest = netObj.GetComponent<PlayerQuest>();
            if (quest != null) quest.UpdateObjective(type, id, amount);
        }
    }

    [ClientRpc]
    void StartBuffsClientRPC(ulong attackerNetworkObjectId, bool hasMight, bool hasSwiftness, bool hasAlacrity, bool hasProtection, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(attackerNetworkObjectId, out NetworkObject attackerObject))
        {
            PlayerStateMachine sm = attackerObject.GetComponent<PlayerStateMachine>();
            if (sm == null) return;

            if (hasMight) sm.Buffs.might.StartMight(1, 30);
            if (hasSwiftness) sm.Buffs.swiftness.StartSwiftness(1, 30);
            if (hasAlacrity) sm.Buffs.alacrity.StartAlacrity(1, 30);
            if (hasProtection) sm.Buffs.protection.StartProtection(1, 30);
        }
    }

    [ClientRpc]
    void DeathClientRpc()
    {
        stateMachine.Collider.enabled = false;
        shadowSprite.enabled = false;
        CastBar.gameObject.SetActive(false);
    }
}
