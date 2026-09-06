using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerInputHandler input;
    public Inventory PlayerInventory;

    [Header("ToolTip")]
    [SerializeField] GameObject toolTipPanel;
    [SerializeField] ToolTip toolTip;

    [Header("UI")]
    //public CastBar CastBar;
    public Image[] playerImages;
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] RectTransform playerUIRect;

    public bool IsInteracting = false;
    public bool CanSellItems = false;
    public bool CanUpgradeItems = false;

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
        stats.OnCharacterDeath.AddListener(ClearTarget);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDeath.RemoveListener(DeathClientRPC);
    }

    private void Update()
    {
        // Only allow the owner of this player object to run this code.
        if (!IsOwner) return;

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //stats.TakeDamage(1, DamageType.Flat, NetworkObject, transform.position);
            stateMachine.Buffs.haste.StartHaste(1, 15);
        }

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.F2))
        {
            stats.GiveHeal(1, HealType.Flat);
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
        stateMachine.SetState(new PlayerDeathState(stateMachine));
    }

    void ClearTarget(NetworkObject attackerID)
    {
        EnemyStateMachine enemy = attackerID.GetComponent<EnemyStateMachine>();
        if (enemy != null)
        {
            if (enemy.SecondTarget != null)
            {
                enemy.Target = enemy.SecondTarget;
                enemy.SecondTarget = null;
                enemy.IsPlayerInRange = true;
            }
            else
            {
                enemy.Target = null;
                enemy.IsPlayerInRange = false;
            }
        }
    }

    public void ShowToolTip(InventorySlotData data)
    {
        toolTipPanel.SetActive(true);
        toolTipPanel.transform.SetAsLastSibling();
        toolTip.GetData(data);
        toolTip.UpdateToolTip();
    }

    public void HideToolTip()
    {
        toolTipPanel.SetActive(false);
    }
}
