using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Player : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Components")]
    public Inventory PlayerInventory;
    public GameObject spawn_Effect;
    public EnduranceBar EnduranceBar;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] HealthBar healthBar;
    [SerializeField] PlayerInitialize playerInitialize;

    [Header("UI")]
    public TextMeshProUGUI CoinText;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] RectTransform playerUIRect;

    [Header("Stats")]
    public float Coins;
    public int hairIndex;

    [Header("Player Stats")]
    public int PlayerLevel;
    public float CurrentExperience;
    public float RequiredExperience;

    [Header("Health")]
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Endurance")]
    public float Endurance;
    public float MaxEndurance;

    [Header("Movement Speed")]
    public float BaseSpeed;
    public float CurrentSpeed;

    [Header("Attack Damage")]
    public int BaseDamage;
    public int CurrentDamage;

    [Header("Attack Speed")]
    public float BaseAttackSpeed;
    public float CurrentAttackSpeed;

    [Header("CDR")]
    public float BaseCDR;
    public float CurrentCDR;

    [Header("Armor")]
    public float BaseArmor;
    public float CurrentArmor;


    public enum PlayerClass
    {
        Beginner,
        Warrior,
        Magician,
        Archer,
        Rogue
    }

    public PlayerClass playerClass;

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
            playerInitialize.LoadPlayerStats();
            Health.Value = MaxHealth.Value;

            Endurance = MaxEndurance;
        }

        Health.OnValueChanged += OnHealthChanged;
        MaxHealth.OnValueChanged += OnMaxHealthChanged;

        if (IsOwner)
        {
            PlayerCamera();
        }

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

        Debug.Log($"Player{attackerID} dealt {finalDamage} to Player{NetworkObject}");

        if (Health.Value <= 0)
        {
            //Die();
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
}
