using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Components")]
    public Inventory PlayerInventory;
    public GameObject spawn_Effect;
    public CastBar CastBar;
    [SerializeField] PlayerSave save;
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

    private void Update()
    {
        if (InCombat)
        {
            CombatTime += Time.deltaTime;

            if (CombatTime >= 10)
            {
                CombatTime = 0;
                InCombat = false;

                /*
                if (Health.Value < MaxHealth.Value)
                {
                    stateMachine.Buffs.regeneration.Regeneration(HealType.Percentage, 10, .5f, 5);
                }
                */
            }
        }
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
        input.cameraInstance = cameraInstance.GetComponent<Camera>();
    }

    [ClientRpc]
    void DeathClientRPC()
    {
        if (!IsOwner) return;
        stateMachine.SetState(PlayerStateMachine.State.Death);
    }
}
