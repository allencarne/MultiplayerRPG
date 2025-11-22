using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : NetworkBehaviour, IDamageable, IHealable
{
    public string Enemy_ID;
    public string Enemy_Name;
    public int Enemy_Level;

    [Header("Health")]
    [SerializeField] float StartingMaxHealth;
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);
    [Header("Speed")]
    public float BaseSpeed;
    public float CurrentSpeed;
    [Header("Damage")]
    public int BaseDamage;
    public int CurrentDamage;
    [Header("AttackSpeed")]
    public float BaseAttackSpeed;
    public float CurrentAttackSpeed;
    [Header("CDR")]
    public float BaseCDR;
    public float CurrentCDR;
    [Header("Armor")]
    public float BaseArmor;
    public float CurrentArmor;

    [Header("Exp")]
    public float expToGive;

    [Header("Components")]
    [HideInInspector] public EnemySpawner EnemySpawnerReference;
    [HideInInspector] public Totem TotemReference;
    [SerializeField] GameObject expPrefab;
    EnemyStateMachine stateMachine;
    [SerializeField] GameObject spawn_Effect;
    [SerializeField] GameObject death_Effect;
    [SerializeField] GameObject death_EffectParticle;
    [SerializeField] HealthBar healthBar;
    public PatienceBar PatienceBar;
    public CastBar CastBar;
    public SpriteRenderer bodySprite;
    public SpriteRenderer shadowSprite;

    [Header("Variables")]
    public float TotalPatience;
    public bool IsDummy;
    public bool IsDead;

    [Header("Events")]
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    private void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);

        // Set Speed
        CurrentSpeed = BaseSpeed;

        // Set Damage
        CurrentDamage = BaseDamage;

        // Set Attack Speed
        CurrentAttackSpeed = BaseAttackSpeed;

        // Set CDR
        CurrentCDR = BaseCDR;

        // Set Armor
        CurrentArmor = BaseArmor;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            MaxHealth.Value = StartingMaxHealth;
            Health.Value = MaxHealth.Value;
        }

        Health.OnValueChanged += OnHealthChanged;
        MaxHealth.OnValueChanged += OnMaxHealthChanged;

        // Initial UI update
        healthBar.UpdateHealthBar(MaxHealth.Value, Health.Value);
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(MaxHealth.Value, newValue);
    }

    private void OnMaxHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(newValue, Health.Value);
    }

    #region Damage/Heal

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;
        if (IsDummy) PatienceBar.Patience.Value = 0;

        TargetAttacker(attackerID);

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        Health.Value = Mathf.Max(Health.Value - roundedDamage, 0);

        // Feedback
        TriggerFlashEffectClientRpc(Color.red);
        OnDamaged?.Invoke(roundedDamage);

        // Quest
        UpdateObjectiveClientRpc(ObjectiveType.Hit, Enemy_ID, 1, attackerID.NetworkObjectId);

        if (Health.Value <= 0)
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
        float armor = CurrentArmor; // Get the target's current armor

        switch (damageType)
        {
            case DamageType.Flat:
                {
                    float armorMultiplier = 100f / (100f + armor); // How much of the damage is applied after armor
                    return baseDamage * armorMultiplier; // Flat base damage reduced by armor
                }

            case DamageType.Percent:
                {
                    float percentDamage = MaxHealth.Value * (baseDamage / 100f); // Calculate % of Max Health as base damage
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
            healAmount = MaxHealth.Value * (healAmount / 100f); // Get %
        }

        Health.Value = Mathf.Min(Health.Value + healAmount, MaxHealth.Value);

        TriggerFlashEffectClientRpc(Color.green);
        OnHealed?.Invoke(healAmount);
    }

    #endregion

    #region Target

    void TargetAttacker(NetworkObject attackerID)
    {
        if (stateMachine.state == EnemyStateMachine.State.Reset) return;

        if (stateMachine.Target == null)
        {
            stateMachine.Target = attackerID.transform;
            stateMachine.IsPlayerInRange = true;
        }
    }

    #endregion

    #region Flash

    public IEnumerator FlashEffect(Color color)
    {
        float flashDuration = 0.1f;

        bodySprite.color = color;
        yield return new WaitForSeconds(flashDuration / 2);

        bodySprite.color = Color.white;
        yield return new WaitForSeconds(flashDuration / 2);

        bodySprite.color = color;
        yield return new WaitForSeconds(flashDuration / 2);

        // Reset to original color
        bodySprite.color = Color.white;
    }

    [ClientRpc]
    public void TriggerFlashEffectClientRpc(Color flashColor)
    {
        StartCoroutine(FlashEffect(flashColor));
    }

    #endregion

    #region RPC

    [ClientRpc]
    private void UpdateObjectiveClientRpc(ObjectiveType type, string id, int amount, ulong attackerNetId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(attackerNetId, out var netObj))
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
        Instantiate(death_Effect, position, rotation);
        Quaternion particleRotation = Quaternion.Euler(-90f, 0f, 0f);
        Instantiate(death_EffectParticle, position, particleRotation);

        stateMachine.EnemyAnimator.Play("Death");
        stateMachine.Collider.enabled = false;
        shadowSprite.enabled = false;
        CastBar.gameObject.SetActive(false);
    }

    #endregion
}
