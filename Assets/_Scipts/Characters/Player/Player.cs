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
    public CastBar CastBar;
    [SerializeField] Rigidbody2D rb;
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

    [Header("Player Stats")]
    public int PlayerLevel;
    public float CurrentExperience;
    public float RequiredExperience;

    [Header("Health")]
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Endurance")]
    public NetworkVariable<float> Endurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxEndurance = new(writePerm: NetworkVariableWritePermission.Server);

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TakeDamage(1, DamageType.Flat, NetworkObject);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GiveHeal(1, HealType.Flat);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerInitialize.LoadPlayerStats();
            Health.Value = MaxHealth.Value;
            Endurance.Value = MaxEndurance.Value;
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
        EnduranceBar.UpdateEnduranceBar(MaxEndurance.Value,Endurance.Value);
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(MaxHealth.Value, newValue);
    }

    private void OnMaxHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(newValue, Health.Value);
    }

    private void OnEnduranceChanged(float oldValue, float newValue)
    {
        EnduranceBar.UpdateEnduranceBar(MaxEndurance.Value, newValue);
    }

    private void OnMaxEnduranceChanged(float oldValue, float newValue)
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

        //Debug.Log($"Player{attackerID} dealt {finalDamage} to Player{NetworkObject}");

        TriggerFlashEffectClientRpc(Color.red);

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

        TriggerFlashEffectClientRpc(Color.green);
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
