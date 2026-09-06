using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [Header("Components")]
    [HideInInspector] public EnemySpawner EnemySpawnerReference;
    [HideInInspector] public Totem TotemReference;
    [SerializeField] GameObject expPrefab;
    [SerializeField] EnemyStateMachine stateMachine;
    public CharacterStats stats;
    public EnemyData Data;

    [Header("Sprites")]
    public SpriteRenderer bodySprite;
    public SpriteRenderer shadowSprite;

    [Header("UI")]
    [SerializeField] HealthBar healthBar;
    public PatienceBar PatienceBar;
    //public CastBar CastBar;

    [Header("Bools")]
    public bool IsDummy;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            stats.net_TotalHP.Value = Data.StartingHealth;
            stats.net_BaseHP.Value = Data.StartingHealth;
            stats.net_CurrentHP.Value = Data.StartingHealth;

            stats.net_BaseSpeed.Value = Data.StartingSpeed;
            stats.net_BaseDamage.Value = Data.StartingDamage;
            stats.net_BaseAS.Value = Data.StartingAS;
            stats.net_BaseCDR.Value = Data.StartingCDR;
            stats.net_BaseArmor.Value = Data.StartingArmor;
        }

        stats.OnCharacterDamaged.AddListener(Damaged);
        stats.OnCharacterDeath.AddListener(Death);

        // Start passive on spawn (no level requirement for enemies)
        if (Data != null && Data.PassiveAbility != null)
        {
            // Use index 0 by convention (you can change if EnemyData supports multiple passives)
            stateMachine.SetPassive(Data.PassiveAbility, 0);
        }
    }

    public override void OnNetworkDespawn()
    {
        // Ensure passive subscriptions are cleaned up
        if (stateMachine != null)
        {
            stateMachine.ClearPassive();
        }

        stats.OnCharacterDamaged.RemoveListener(Damaged);
        stats.OnCharacterDeath.RemoveListener(Death);
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
        if (quest != null) UpdateObjectiveClientRpc(ObjectiveType.Hit, Data.Enemy_ID, 1, attackerID.NetworkObjectId, rpcParams);

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
        if (exp) exp.IncreaseEXP(Data.ExpToGive);

        ulong targetClientId = attackerID.OwnerClientId;
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { targetClientId }
            }
        };

        PlayerQuest quest = attackerID.GetComponent<PlayerQuest>();
        if (quest != null) UpdateObjectiveClientRpc(ObjectiveType.Kill, Data.Enemy_ID, 1, attackerID.NetworkObjectId, rpcParams);

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
        ClearTarget(attackerID);

        DeathClientRpc();
        stateMachine.SetState(new EnemyDeathState(stateMachine));
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
        if (stateMachine.state is EnemyResetState) return;

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

    void ClearTarget(NetworkObject attackerID)
    {
        EnemyStateMachine enemy = attackerID.GetComponent<EnemyStateMachine>();
        if (enemy != null)
        {
            if (enemy.SecondTarget != null)
            {
                enemy.Target = enemy.SecondTarget;
                enemy.SecondTarget = null;
                enemy.IsPlayerInRange = true;
            }
            else
            {
                enemy.Target = null;
                enemy.IsPlayerInRange = false;
            }
        }
    }

    IEnumerator DropEXP(Transform attackerPosition)
    {
        for (int i = 0; i < Data.ExpToGive; i++)
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
        stateMachine.Collider2D.enabled = false;
        shadowSprite.enabled = false;
        stateMachine.CastBar.gameObject.SetActive(false);
    }
}
