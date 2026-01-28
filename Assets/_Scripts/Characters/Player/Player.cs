using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Combat")]
    public NetworkVariable<bool> InCombat = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private NetworkVariable<float> CombatTime = new NetworkVariable<float>(0f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    bool IsRegen = false;

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

    public bool IsDead = false;
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

        if (IsServer)
        {
            stats.OnDamaged.AddListener(TakeDamage);
            stats.OnDamageDealt.AddListener(DealDamage);
        }
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDeath.RemoveListener(DeathClientRPC);

        if (IsServer)
        {
            stats.OnDamaged.RemoveListener(TakeDamage);
            stats.OnDamageDealt.RemoveListener(DealDamage);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            stateMachine.Buffs.regeneration.StartRegen(1,5);
        }

        if (IsServer && InCombat.Value)
        {
            CombatTime.Value += Time.deltaTime;
            if (CombatTime.Value >= 10)
            {
                InCombat.Value = false;
                CombatTime.Value = 0;
            }
        }

        // Each client handles their own regen state locally
        if (!IsOwner) return;

        // Start regen when out of combat and missing health
        if (!InCombat.Value && !IsRegen && stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
        {
            IsRegen = true;
            stateMachine.Buffs.regeneration.StartRegen(1, -1);
        }

        // Stop regen when full health or back in combat
        if (IsRegen && (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value || InCombat.Value))
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
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
        CombatTime.Value = 0;
    }

    void DealDamage()
    {
        if (!IsServer) return;
        InCombat.Value = true;
        CombatTime.Value = 0;
    }
}
