using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour, IDamageable, IHealable
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


    public override void OnNetworkSpawn()
    {
        //stats.OnDeath.AddListener(DeathState);
    }

    public override void OnNetworkDespawn()
    {
        //stats.OnDeath.RemoveListener(DeathState);
    }

    private void Start()
    {
        if (IsServer)
        {
            stats.MaxHealth.Value = startingHealth;
            stats.Health.Value = stats.MaxHealth.Value;
        }
    }

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;
        if (IsDummy) PatienceBar.Patience.Value = 0;

        TargetAttacker(attackerID);

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        stats.Health.Value = Mathf.Max(stats.Health.Value - roundedDamage, 0);

        // Quest
        UpdateObjectiveClientRpc(ObjectiveType.Hit, Enemy_ID, 1, attackerID.NetworkObjectId);

        if (stats.Health.Value <= 0)
        {
            if (IsDummy) return;

            Player player = attackerID.GetComponent<Player>();
            if (player != null && TotemReference != null)
            {
                switch (TotemReference.CurrentEvent)
                {
                    case SwarmEvent swarm: swarm.EnemyDeath(player); break;
                    case BossEvent boss: boss.EnemyDeath(player); break;
                }
            }

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

            Transform attackerPosition = attackerID.GetComponent<Transform>();
            if (attackerPosition != null) StartCoroutine(delay(attackerPosition));

            PlayerExperience exp = attackerID.gameObject.GetComponent<PlayerExperience>();
            if (exp) exp.IncreaseEXP(expToGive);

            UpdateObjectiveClientRpc(ObjectiveType.Kill, Enemy_ID, 1, attackerID.NetworkObjectId);

            DeathClientRpc(transform.position, transform.rotation);
            stateMachine.SetState(EnemyStateMachine.State.Death);
        }
    }

    IEnumerator delay(Transform attackerPosition)
    {
        for (int i = 0; i < expToGive; i++)
        {
            GameObject expObj = Instantiate(expPrefab, transform.position, transform.rotation);
            expObj.GetComponent<TravelToTarget>().target = attackerPosition;
            yield return new WaitForSeconds(.1f);
        }
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType)
    {
        float armor = stats.Armor; // Get the target's current armor

        switch (damageType)
        {
            case DamageType.Flat:
                {
                    float armorMultiplier = 100f / (100f + armor); // How much of the damage is applied after armor
                    return baseDamage * armorMultiplier; // Flat base damage reduced by armor
                }

            case DamageType.Percent:
                {
                    float percentDamage = stats.MaxHealth.Value * (baseDamage / 100f); // Calculate % of Max Health as base damage
                    float armorMultiplier = 100f / (100f + armor); // Still apply armor reduction
                    return percentDamage * armorMultiplier; // % Health damage reduced by armor
                }

            case DamageType.True:
                {
                    return baseDamage; // Ignore Armor
                }

            default:
                {
                    return baseDamage; // Fallback
                }
        }
    }

    public void GiveHeal(float healAmount, HealType healType)
    {
        if (!IsServer) return;

        if (healType == HealType.Percentage)
        {
            healAmount = stats.MaxHealth.Value * (healAmount / 100f); // Get %
        }

        stats.Health.Value = Mathf.Min(stats.Health.Value + healAmount, stats.MaxHealth.Value);
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
    private void DeathClientRpc(Vector3 position, Quaternion rotation)
    {
        stateMachine.EnemyAnimator.Play("Death");
        stateMachine.Collider.enabled = false;
        shadowSprite.enabled = false;
        CastBar.gameObject.SetActive(false);
    }
}
