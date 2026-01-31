using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Combat")]
    public NetworkVariable<bool> InCombat = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    float combatTime = 0f;
    bool IsRegen = false;
    Coroutine combatTimerCoroutine;

    [Header("Components")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerInputHandler input;
    [SerializeField] PlayerSave save;
    public Inventory PlayerInventory;

    [Header("Sprites")]
    public SpriteRenderer PlayerHeadSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer SwordSprite;
    public SpriteRenderer ShadowSprite;
    public SpriteRenderer AimerSprite;

    [Header("UI")]
    public CastBar CastBar;
    public Image[] playerImages;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] RectTransform playerUIRect;

    public bool IsInteracting = false;
    public bool CanSellItems = false;

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

    public override void OnNetworkSpawn()
    {
        if (IsOwner) PlayerCamera();
        stats.OnDeath.AddListener(DeathClientRPC);
        InCombat.OnValueChanged += OnCombatStateChanged;

        if (IsOwner)
        {
            stats.net_CurrentHP.OnValueChanged += OnHPChanged;
            stats.net_TotalHP.OnValueChanged += OnMaxHPChanged;
        }

        if (IsServer)
        {
            stats.OnDamaged.AddListener(TakeDamage);
            stats.OnDamageDealt.AddListener(DealDamage);
        }
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDeath.RemoveListener(DeathClientRPC);
        InCombat.OnValueChanged -= OnCombatStateChanged;

        if (IsOwner)
        {
            stats.net_CurrentHP.OnValueChanged -= OnHPChanged;
            stats.net_TotalHP.OnValueChanged -= OnMaxHPChanged;
        }

        if (IsServer)
        {
            stats.OnDamaged.RemoveListener(TakeDamage);
            stats.OnDamageDealt.RemoveListener(DealDamage);

            if (combatTimerCoroutine != null)
            {
                StopCoroutine(combatTimerCoroutine);
            }
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
        input.cameraInstance = cameraInstance.GetComponent<Camera>();
    }

    [ClientRpc]
    void DeathClientRPC()
    {
        stateMachine.SetState(PlayerStateMachine.State.Death);
    }

    void TakeDamage(float damage)
    {
        if (!IsServer) return;
        InCombat.Value = true;
        combatTime = 0;
    }

    void DealDamage()
    {
        if (!IsServer) return;
        InCombat.Value = true;
        combatTime = 0;
    }

    void OnCombatStateChanged(bool previousValue, bool newValue)
    {
        if (IsServer)
        {
            if (newValue)
            {
                // Entered combat - start timer
                if (combatTimerCoroutine != null)
                {
                    StopCoroutine(combatTimerCoroutine);
                }
                combatTimerCoroutine = StartCoroutine(CombatTimer());
            }
            else
            {
                // Exited combat - stop timer
                if (combatTimerCoroutine != null)
                {
                    StopCoroutine(combatTimerCoroutine);
                    combatTimerCoroutine = null;
                }
                combatTime = 0;
            }
        }

        // Handle on owning client for regeneration
        if (IsOwner)
        {
            HandleRegeneration(newValue);
        }
    }

    IEnumerator CombatTimer()
    {
        combatTime = 0f;

        while (combatTime < 10f)
        {
            combatTime += Time.deltaTime;
            yield return null;
        }

        // Timer expired, exit combat
        InCombat.Value = false;
        combatTimerCoroutine = null;
    }

    void HandleRegeneration(bool inCombat)
    {
        if (!inCombat && !IsRegen && stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
        {
            IsRegen = true;
            stateMachine.Buffs.regeneration.StartRegen(1, -1);
        }
        else if (IsRegen && (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value || inCombat))
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }

    void OnHPChanged(float previousValue, float newValue)
    {
        if (IsOwner)
        {
            UpdateRegeneration();
        }
    }

    void OnMaxHPChanged(float previousValue, float newValue)
    {
        if (IsOwner)
        {
            UpdateRegeneration();
        }
    }

    void UpdateRegeneration()
    {
        bool shouldRegen = !InCombat.Value && stats.net_CurrentHP.Value < stats.net_TotalHP.Value;

        if (shouldRegen && !IsRegen)
        {
            // Start regeneration
            IsRegen = true;
            stateMachine.Buffs.regeneration.StartRegen(1, -1);
        }
        else if (!shouldRegen && IsRegen)
        {
            // Stop regeneration
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }
}
