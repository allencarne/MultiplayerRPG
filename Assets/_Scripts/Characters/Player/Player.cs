using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
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
    public bool InCombat = false;
    public bool IsInteracting = false;
    public bool CanSellItems = false;
    bool IsRegen = false;
    float CombatTime = 0;

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
        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);
        stats.OnDeath.AddListener(DeathClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(TakeDamage);
        stats.OnDamageDealt.RemoveListener(DealDamage);
        stats.OnDeath.RemoveListener(DeathClientRPC);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            stateMachine.DeBuffs.bleed.StartBleed(1, 5);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            stateMachine.DeBuffs.bleed.StartBleed(1,-1);
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            stateMachine.DeBuffs.bleed.StartBleed(-1,-1);
        }

        if (InCombat)
        {
            CombatTime += Time.deltaTime;

            if (CombatTime >= 10)
            {
                InCombat = false;
                CombatTime = 0;

                if (stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
                {
                    IsRegen = true;
                    stateMachine.Buffs.regeneration.StartRegen(1, -1);
                }
            }
        }

        if (!IsRegen) return;
        if (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value || InCombat)
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
        InCombat = true;
        CombatTime = 0;
    }

    void DealDamage(float damage, Vector2 position)
    {
        InCombat = true;
        CombatTime = 0;
    }
}
