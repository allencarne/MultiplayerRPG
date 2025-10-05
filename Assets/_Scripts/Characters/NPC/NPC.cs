using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NPC : NetworkBehaviour, IDamageable, IHealable
{
    public string NPC_ID;
    public string NPC_Name;
    public int NPC_Level;


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

    [Header("Customization")]
    [SerializeField] Color skinColor;
    [SerializeField] Color hairColor;
    public int hairIndex;
    public int HeadIndex;
    public int ChestIndex;
    public int LegsIndex;

    [Header("Variables")]
    public float TotalPatience;
    public bool IsDead;

    [Header("Sprites")]
    public SpriteRenderer SwordSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer HairSprite;
    public SpriteRenderer EyeSprite;
    public SpriteRenderer ShadowSprite;

    [Header("Components")]

    public CastBar CastBar;
    [SerializeField] HealthBar healthBar;
    [SerializeField] NPCStateMachine stateMachine;
    [SerializeField] GameObject death_Effect;
    public PatienceBar PatienceBar;
    public GameObject spawn_Effect;

    [Header("Events")]
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    public enum Type
    {
        Quest,
        Vendor,
        Villager,
        Guard,
        Patrol
    }

    public Type type;


    private void Start()
    {
        BodySprite.color = skinColor;
        HairSprite.color = hairColor;

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

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        TargetAttacker(attackerID);

        // Calculate
        float finalDamage = CalculateFinalDamage(damage, damageType);
        int roundedDamage = Mathf.RoundToInt(finalDamage);

        // Subtract
        Health.Value = Mathf.Max(Health.Value - roundedDamage, 0);

        // Feedback
        TriggerFlashEffectClientRpc();
        OnDamaged?.Invoke(roundedDamage);

        if (Health.Value <= 0)
        {
            HandleDeathClientRPC();

            stateMachine.Target = null;
            EnemyStateMachine enemy = attackerID.GetComponent<EnemyStateMachine>();
            if (enemy != null) enemy.Target = null;
        }
    }

    [ClientRpc]
    void HandleDeathClientRPC()
    {
        if (!IsOwner) return;
        stateMachine.Death();
        Instantiate(death_Effect, transform.position, transform.rotation);
    }

    void TargetAttacker(NetworkObject attackerID)
    {
        if (!attackerID.gameObject.CompareTag("Enemy")) return;

        if (stateMachine.Target == null)
        {
            stateMachine.Target = attackerID.transform;
            stateMachine.IsEnemyInRange = true;
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

        TriggerFlashEffectClientRpc();
        OnHealed?.Invoke(healAmount);
    }

    public IEnumerator FlashEffect()
    {
        BodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        // Reset to original color
        BodySprite.color = skinColor;
    }

    [ClientRpc]
    public void TriggerFlashEffectClientRpc()
    {
        StartCoroutine(FlashEffect());
    }
}
