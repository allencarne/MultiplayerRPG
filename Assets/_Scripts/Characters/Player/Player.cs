using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : NetworkBehaviour, IDamageable, IHealable
{
    [Header("Components")]
    public Inventory PlayerInventory;
    public GameObject spawn_Effect;
    public CastBar CastBar;
    [SerializeField] PlayerInitialize playerInitialize;
    [SerializeField] PlayerInputHandler input;

    [Header("Sprites")]
    public SpriteRenderer SwordSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer EyeSprite;
    public SpriteRenderer HairSprite;
    public SpriteRenderer ShadowSprite;
    public SpriteRenderer AimerSprite;

    [Header("UI")]
    public Image[] playerImages;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] HealthBar healthBar;
    [SerializeField] FuryBar furyBar;
    public EnduranceBar EnduranceBar;
    public TextMeshProUGUI CoinText;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] RectTransform playerUIRect;

    public bool IsDead = false;
    public bool InCombat = false;
    public float CombatTime = 0;
    public bool IsInteracting = false;

    [Header("Ability Indexes")]
    public int FirstPassiveIndex = 0;
    public int SecondPassiveIndex = -1;
    public int ThirdPassiveIndex = -1;
    public int BasicIndex = 0;
    public int OffensiveIndex = -1;
    public int MobilityIndex = -1;
    public int DefensiveIndex = -1;
    public int UtilityIndex = -1;
    public int UltimateIndex = -1;

    [Header("Stats")]
    public float Coins;
    public int hairIndex;

    // BASE STATS CHANGE WITH LEVEL/EQUIPMENT -- PERMANENT
    // CURRENT STATS CHANGE WITH BUFFS/DEBUFFS -- TEMPORARY

    [Header("Player Stats")]
    public string PlayerName;
    public NetworkVariable<int> PlayerLevel = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentExperience = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> RequiredExperience = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> AttributePoints = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Health")]
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Fury")]
    public NetworkVariable<float> Fury = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxFury = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Endurance")]
    public NetworkVariable<float> Endurance = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxEndurance = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Movement Speed")]
    public NetworkVariable<float> BaseSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> CurrentSpeed = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Attack Damage")]
    public NetworkVariable<int> BaseDamage = new(writePerm: NetworkVariableWritePermission.Server);
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

    private void Update()
    {
        if (InCombat)
        {
            CombatTime += Time.deltaTime;

            if (CombatTime >= 10)
            {
                CombatTime = 0;
                InCombat = false;

                if (Health.Value < MaxHealth.Value)
                {
                    //stateMachine.Buffs.regeneration.Regeneration(HealType.Percentage, 10, .5f, 5);
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerInitialize.LoadPlayerStats();
        }

        Health.OnValueChanged += OnHealthChanged;
        MaxHealth.OnValueChanged += OnMaxHealthChanged;

        Fury.OnValueChanged += OnFuryChanged;
        MaxFury.OnValueChanged += OnMaxFuryChanged;

        Endurance.OnValueChanged += OnEnduranceChanged;
        MaxEndurance.OnValueChanged += OnMaxEnduranceChanged;

        if (IsOwner)
        {
            PlayerCamera();
        }

        // Initial UI update
        healthBar.UpdateHealthBar(MaxHealth.Value, Health.Value);
        furyBar.UpdateFuryBar(MaxFury.Value, Fury.Value);
        EnduranceBar.UpdateEnduranceBar(MaxEndurance.Value, Endurance.Value);
    }

    void OnHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(MaxHealth.Value, newValue);
    }

    void OnMaxHealthChanged(float oldValue, float newValue)
    {
        healthBar.UpdateHealthBar(newValue, Health.Value);
    }

    void OnFuryChanged(float oldValue, float newValue)
    {
        furyBar.UpdateFuryBar(MaxFury.Value, newValue);
    }

    void OnMaxFuryChanged(float oldValue, float newValue)
    {
        furyBar.UpdateFuryBar(newValue, Fury.Value);
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
        input.cameraInstance = cameraInstance.GetComponent<Camera>();
    }

    public void CoinCollected(float amount)
    {
        Coins += amount;
        CoinText.text = $"{Coins}<sprite index=0>";

        playerInitialize.SaveStats();
    }

    #region Damage/Heal

    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {
        if (!IsServer) return;

        CombatTime = 0;
        InCombat = true;

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
            DeathClientRPC();

            EnemyStateMachine enemy = attackerID.GetComponent<EnemyStateMachine>();
            if (enemy != null) enemy.Target = null;
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
            healAmount = MaxHealth.Value * (healAmount / 100f);
        }

        // Heal
        float missingHealth = MaxHealth.Value - Health.Value;
        float actualHeal = Mathf.Min(healAmount, missingHealth);
        int roundedHeal = Mathf.FloorToInt(actualHeal);

        Health.Value += roundedHeal;

        // Feedback
        TriggerFlashEffectClientRpc();
        OnHealed?.Invoke(roundedHeal);
    }

    #endregion

    #region Death

    [ClientRpc]
    void DeathClientRPC()
    {
        if (!IsOwner) return;
        stateMachine.SetState(PlayerStateMachine.State.Death);
    }

    #endregion

    #region Attribute Points

    public void ConsumeAttributePoints(int amount)
    {
        if (IsServer)
        {
            AttributePoints.Value -= amount;
        }
        else
        {
            ConsumeAttributePointsServerRPC(amount);
        }
    }

    [ServerRpc]
    void ConsumeAttributePointsServerRPC(int amount)
    {
        AttributePoints.Value -= amount;
    }

    #endregion

    #region Increase Stats

    public void IncreaseHealth(float amount)
    {
        if (IsServer)
        {
            MaxHealth.Value += amount;
            Health.Value += amount;
        }
        else
        {
            IncreaseHealthServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseHealthServerRPC(float amount)
    {
        MaxHealth.Value += amount;
        Health.Value += amount;
    }

    public void IncreaseDamage(int amount)
    {
        if (IsServer)
        {
            BaseDamage.Value += amount;
            CurrentDamage.Value += amount;
        }
        else
        {
            IncreaseDamageServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseDamageServerRPC(int amount)
    {
        BaseDamage.Value += amount;
        CurrentDamage.Value += amount;
    }

    public void IncreaseAttackSpeed(float amount)
    {
        if (IsServer)
        {
            BaseAttackSpeed.Value += amount;
            CurrentAttackSpeed.Value += amount;
        }
        else
        {
            IncreaseAttackSpeedServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseAttackSpeedServerRPC(float amount)
    {
        BaseAttackSpeed.Value += amount;
        CurrentAttackSpeed.Value += amount;
    }

    public void IncreaseCoolDown(float amount)
    {
        if (IsServer)
        {
            BaseCDR.Value += amount;
            CurrentCDR.Value += amount;
        }
        else
        {
            IncreaseCoolDownServerRPC(amount);
        }
    }

    [ServerRpc]
    void IncreaseCoolDownServerRPC(float amount)
    {
        BaseCDR.Value += amount;
        CurrentCDR.Value += amount;
    }

    #endregion

    #region Flash

    public IEnumerator FlashEffect()
    {
        BodySprite.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        // Reset to original color
        BodySprite.color = playerInitialize.net_bodyColor.Value;
    }

    [ClientRpc]
    public void TriggerFlashEffectClientRpc()
    {
        StartCoroutine(FlashEffect());
    }

    #endregion
}
