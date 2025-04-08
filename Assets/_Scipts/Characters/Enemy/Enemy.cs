using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [SerializeField] EnemyHealthBar healthBar;
    public Image CaseBar;

    [HideInInspector] public EnemySpawner EnemySpawnerReference;
    public Image PatienceBar;
    public float TotalPatience;
    public float CurrentPatience;

    [SerializeField] UnityEvent OnDeath;
    [SerializeField] UnityEvent OnDamage;

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
        healthBar.UpdateHealthUI(MaxHealth.Value, Health.Value);
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthUI(MaxHealth.Value, newValue);
    }

    private void OnMaxHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthUI(newValue, Health.Value);
    }

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        OnDamage?.Invoke();

        float finalDamage = 0f;

        if (damageType == DamageType.Flat)
        {
            finalDamage = Mathf.Max(damage - CurrentArmor, 0);
        }
        else if (damageType == DamageType.Percentage)
        {
            finalDamage = MaxHealth.Value * (damage / 100f); // Percentage-based damage ignores armor
        }

        Health.Value = Mathf.Max(Health.Value - finalDamage, 0);

        Debug.Log($"Player{attackerID} dealt {finalDamage} to Enemy{NetworkObject}");

        if (Health.Value <= 0)
        {
            OnDeath?.Invoke();
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
    }

    public void UpdatePatienceBar(float patience)
    {
        if (PatienceBar != null)
        {
            // Calculate the fill amount
            float fillAmount = Mathf.Clamp01(patience / TotalPatience);

            // Update the patience bar fill amount
            PatienceBar.fillAmount = fillAmount;
        }
    }
}
