using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class Player : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Components")]
    public Inventory PlayerInventory;
    public GameObject spawn_Effect;
    public EnduranceBar EnduranceBar;
    public CastBar CastBar;
    [SerializeField] HealthBar healthBar;
    [SerializeField] PlayerInitialize playerInitialize;
    [SerializeField] SpriteRenderer bodySprite;

    [Header("UI")]
    public TextMeshProUGUI CoinText;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] RectTransform playerUIRect;

    [Header("Stats")]
    public float Coins;
    public int hairIndex;

    // BASE STATS CHANGE WITH LEVEL/EQUIPMENT -- PERMANENT
    // CURRENT STATS CHANGE WITH BUFFS/DEBUFFS -- TEMPORARY

    [Header("Player Stats")]
    public string PlayerName;
    public int PlayerLevel;
    public float CurrentExperience;
    public float RequiredExperience;
    public float AttributePoints;

    [Header("Health")]
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Endurance")]
    public NetworkVariable<float> Endurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxEndurance = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Movement Speed")]
    public NetworkVariable<float> BaseSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentSpeed = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Attack Damage")]
    public NetworkVariable<int> BaseDamage = new (writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> CurrentDamage = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Attack Speed")]
    public NetworkVariable<float> BaseAttackSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentAttackSpeed = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("CDR")]
    public NetworkVariable<float> BaseCDR = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentCDR = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Armor")]
    public NetworkVariable<float> BaseArmor = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentArmor = new(writePerm: NetworkVariableWritePermission.Server);

    public UnityEvent<float> OnDamaged;
    public UnityEvent<float> OnHealed;

    public enum PlayerClass
    {
        Beginner,
        Warrior,
        Magician,
        Archer,
        Rogue
    }

    public PlayerClass playerClass;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerInitialize.LoadPlayerStats();
        }

        Health.OnValueChanged += OnHealthChanged;
        MaxHealth.OnValueChanged += OnMaxHealthChanged;

        Endurance.OnValueChanged += OnEnduranceChanged;
        MaxEndurance.OnValueChanged += OnMaxEnduranceChanged;

        if (IsOwner)
        {
            PlayerCamera();
        }

        // Initial UI update
        healthBar.UpdateHealthBar(MaxHealth.Value, Health.Value);
        EnduranceBar.UpdateEnduranceBar(MaxEndurance.Value, Endurance.Value);
    }

    private void Start()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);
    }

    void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(MaxHealth.Value, newValue);
    }

    void OnMaxHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(newValue, Health.Value);
    }

    void OnEnduranceChanged(float oldValue, float newValue)
    {
        EnduranceBar.UpdateEnduranceBar(MaxEndurance.Value, newValue);
    }

    void OnMaxEnduranceChanged(float oldValue, float newValue)
    {
        EnduranceBar.UpdateEnduranceBar(newValue, Endurance.Value);
    }

    void PlayerCamera()
    {
        GameObject cameraInstance = Instantiate(cameraPrefab);
        CameraFollow cameraFollow = cameraInstance.GetComponent<CameraFollow>();
        cameraFollow.playerTransform = transform;
        cameraInstance.GetComponent<CameraZoom>().inputHandler = gameObject.GetComponent<PlayerInputHandler>();
        cameraInstance.GetComponent<CameraZoom>().GetPlayer();
        playerUI.worldCamera = cameraInstance.GetComponent<Camera>();
    }

    public void CoinCollected(float amount)
    {
        Coins += amount;
        CoinText.text = Coins.ToString();
        playerInitialize.SavePlayerStats();
    }

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        // Calculate how much damage should actually be applied after defenses.
        float finalDamage = CalculateFinalDamage(damage, damageType);

        // Subtract final damage from health, but don't let health go below 0.
        Health.Value = Mathf.Max(Health.Value - finalDamage, 0);
        
        // Feedback
        TriggerFlashEffectClientRpc();
        OnDamaged?.Invoke(finalDamage);

        if (Health.Value <= 0)
        {
            // Die();
        }
    }

    private float CalculateFinalDamage(float baseDamage, DamageType damageType)
    {
        float armor = CurrentArmor.Value; // Get the target's current armor

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

        // Feedback
        TriggerFlashEffectClientRpc();
        OnHealed?.Invoke(healAmount);
    }

    public IEnumerator FlashEffect()
    {
        bodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        // Reset to original color
        bodySprite.color = playerInitialize.net_bodyColor.Value;
    }

    [ClientRpc]
    public void TriggerFlashEffectClientRpc()
    {
        StartCoroutine(FlashEffect());
    }
}
