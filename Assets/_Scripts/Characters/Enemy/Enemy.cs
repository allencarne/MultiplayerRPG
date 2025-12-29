using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [Header("Variables")]
    public string Enemy_ID;
    public string Enemy_Name;
    public int Enemy_Level;
    public float expToGive;
    public float TotalPatience;
    [SerializeField] float startingHealth;

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
    public bool IsDead;
    public bool IsRegen;

    public override void OnNetworkSpawn()
    {
        stats.OnEnemyDamaged.AddListener(Damaged);
        stats.OnEnemyDeath.AddListener(Death);

        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);

        Invoke("AssignHealth", 1);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnEnemyDamaged.RemoveListener(Damaged);
        stats.OnEnemyDeath.RemoveListener(Death);

        stats.OnDamaged.RemoveListener(TakeDamage);
        stats.OnDamageDealt.RemoveListener(DealDamage);
    }

    void TakeDamage(float damage)
    {
        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    void DealDamage(float damage, Vector2 position)
    {
        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    private void Update()
    {
        if (!IsRegen) return;
        if (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value)
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }

    void AssignHealth()
    {
        if (IsServer)
        {
            stats.net_TotalHP.Value = startingHealth;
            stats.net_BaseHP.Value = startingHealth;
            stats.net_CurrentHP.Value = startingHealth;
        }
    }

    void Damaged(NetworkObject attackerID)
    {
        if (!IsServer) return;
        if (IsDummy) PatienceBar.Patience.Value = 0;

        TargetAttacker(attackerID);
        EventParticipate(attackerID);

        UpdateObjectiveClientRpc(ObjectiveType.Hit, Enemy_ID, 1, attackerID.NetworkObjectId);
    }

    void Death(NetworkObject attackerID)
    {
        if (IsDummy) return;

        EventDeath(attackerID);
        NPCEventParticipation(attackerID);

        Transform attackerPosition = attackerID.GetComponent<Transform>();
        if (attackerPosition != null) StartCoroutine(DropEXP(attackerPosition));

        PlayerExperience exp = attackerID.gameObject.GetComponent<PlayerExperience>();
        if (exp) exp.IncreaseEXP(expToGive);

        UpdateObjectiveClientRpc(ObjectiveType.Kill, Enemy_ID, 1, attackerID.NetworkObjectId);

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
    private void UpdateObjectiveClientRpc(ObjectiveType type, string id, int amount, ulong attackerNetId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(attackerNetId, out NetworkObject netObj))
        {
            PlayerQuest quest = netObj.GetComponent<PlayerQuest>();
            if (quest != null)
            {
                quest.UpdateObjective(type, id, amount);
            }
        }
    }

    [ClientRpc]
    private void DeathClientRpc()
    {
        stateMachine.EnemyAnimator.Play("Death");
        stateMachine.Collider.enabled = false;
        shadowSprite.enabled = false;
        CastBar.gameObject.SetActive(false);
    }
}
