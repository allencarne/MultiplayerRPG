using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health")]
    public float Health;
    public float MaxHealth;
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

    public Image PatienceBar;
    public float TotalPatience;
    public float CurrentPatience;

    [SerializeField] UnityEvent OnDeath;

    private void Start()
    {
        // Set Health
        Health = MaxHealth;

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

    public void TakeDamage(float damage, DamageType damageType, ulong attackerClientId)
    {
        float finalDamage = 0f;

        if (damageType == DamageType.Flat)
        {
            finalDamage = Mathf.Max(damage - CurrentArmor, 0); // Reduce damage by armor
        }
        else if (damageType == DamageType.Percentage)
        {
            finalDamage = MaxHealth * (damage / 100f); // Percentage-based damage ignores armor
        }

        Health = Mathf.Max(Health - finalDamage, 0);
        healthBar.UpdateHealth(Health);

        if (Health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void GiveHeal(float healAmount, HealType healType)
    {
        if (healType == HealType.Percentage)
        {
            healAmount = MaxHealth * (healAmount / 100f); // Get %
        }

        Health = Mathf.Min(Health + healAmount, MaxHealth);
        healthBar.UpdateHealth(Health);
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
