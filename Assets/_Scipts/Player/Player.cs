using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.Events;

public class Player : NetworkBehaviour, IDamageable
{
    public GameObject AttackPrefab;

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

    [Header("Events")]
    public UnityEvent OnDamageTaken;

    [Header("Stats")]
    public float Coins;
    public int hairIndex;

    [Header("Player Stats")]
    public int PlayerLevel;
    public float CurrentExperience;
    public float RequiredExperience;

    [Header("Health")]
    public float Health;
    public float MaxHealth;

    [Header("Endurance")]
    public float Endurance;
    public float MaxEndurance;

    [Header("Movement Speed")]
    public float Speed;
    public float CurrentSpeed;

    [Header("Attack Damage")]
    public int Damage;
    public int CurrentDamage;

    [Header("Attack Speed")]
    public float AttackSpeed;
    public float CurrentAttackSpeed;

    [Header("CDR")]
    public float CDR;
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

        Health = MaxHealth;

        healthBar.UpdateHealthBar(MaxHealth, Health);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerCamera();
        }
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

    public void TakeDamage(float damage)
    {
        // Calculate damage after applying armor
        float damageAfterArmor = Mathf.Max(damage - CurrentArmor, 0);

        // Apply the reduced damage
        Health -= damageAfterArmor;

        // Update Healthbar
        OnDamageTaken?.Invoke();

        // Enter Combat
    }
}
