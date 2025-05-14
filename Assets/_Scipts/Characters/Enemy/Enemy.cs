using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : NetworkBehaviour, IDamageable, IHealable
{
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

    [Header("UI")]
    [SerializeField] HealthBar healthBar;
    public CastBar CastBar;
    [HideInInspector] public EnemySpawner EnemySpawnerReference;
    public SpriteRenderer bodySprite;
    public SpriteRenderer shadowSprite;
    public PatienceBar PatienceBar;
    public float TotalPatience;
    public bool isDummy;
    public bool isDead;

    [Header("Events")]
    [SerializeField] UnityEvent OnDeath;
    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    private void Start()
    {
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
        if (isDummy) PatienceBar.Patience.Value = 0;

        // Calculate how much damage should actually be applied after defenses.
        float finalDamage = CalculateFinalDamage(damage, damageType);

        // Subtract final damage from health, but don't let health go below 0.
        Health.Value = Mathf.Max(Health.Value - finalDamage, 0);

        // Feedback
        TriggerFlashEffectClientRpc(Color.red);
        OnDamaged?.Invoke(finalDamage);

        if (Health.Value <= 0)
        {
            if (isDummy) return;

            PlayerExperience exp = attackerID.gameObject.GetComponent<PlayerExperience>();
            if (exp)
            {
                exp.IncreaseEXP(expToGive);
            }

            OnDeath?.Invoke();
            isDead = true;
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
}
