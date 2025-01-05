using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    [Header("Components")]
    public Inventory PlayerInventory;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject spawn_Effect;
    [SerializeField] EnduranceBar enduranceBar;
    [SerializeField] PlayerInitialize playerInitialize;

    [Header("UI")]
    [SerializeField] Canvas playerUI;
    [SerializeField] GameObject cameraPrefab;
    public TextMeshProUGUI CoinText;
    [SerializeField] RectTransform playerUIRect;

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

    [Header("Endurance (NOT SAVED)")]
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

    [Header("Network Variables")]
    private NetworkVariable<float> net_endurance = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

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

        Endurance = MaxEndurance;

        // Set Endurance Bar UI
        enduranceBar.UpdateEnduranceBar(MaxEndurance, Endurance);
    }

    public override void OnNetworkSpawn()
    {
        net_endurance.OnValueChanged += OnEnduranceChanged;

        if (IsOwner)
        {
            PlayerCamera();
            //AssignPlayerCamera();

            // Initialize endurance network variable
            net_endurance.Value = Endurance;
        }
        else
        {

        }

        // Sync UI for non-owners
        enduranceBar.UpdateEnduranceBar(MaxEndurance, net_endurance.Value);
    }

    public override void OnDestroy()
    {
        net_endurance.OnValueChanged -= OnEnduranceChanged;
    }

    void AssignPlayerCamera()
    {
        // Assign the camera to follow this player
        if (Camera.main.GetComponent<CameraFollow>().playerTransform == null)
        {
            Camera.main.GetComponent<CameraFollow>().playerTransform = transform;
            Camera.main.GetComponent<CameraZoom>().inputHandler = gameObject.GetComponent<PlayerInputHandler>();
            Camera.main.GetComponent<CameraZoom>().GetPlayer();
        }
        else
        {
            GameObject cameraInstance = Instantiate(cameraPrefab);
            CameraFollow cameraFollow = cameraInstance.GetComponent<CameraFollow>();
            cameraFollow.playerTransform = transform;
            cameraInstance.GetComponent<CameraZoom>().inputHandler = gameObject.GetComponent<PlayerInputHandler>();
            cameraInstance.GetComponent<CameraZoom>().GetPlayer();
        }
    }

    void PlayerCamera()
    {
        GameObject cameraInstance = Instantiate(cameraPrefab);
        CameraFollow cameraFollow = cameraInstance.GetComponent<CameraFollow>();
        cameraFollow.playerTransform = transform;
        cameraInstance.GetComponent<CameraZoom>().inputHandler = gameObject.GetComponent<PlayerInputHandler>();
        cameraInstance.GetComponent<CameraZoom>().GetPlayer();
    }

    public void UpdateEndurance(float amount)
    {
        if (IsOwner)
        {
            Endurance -= amount;

            net_endurance.Value = Endurance;

            enduranceBar.UpdateEnduranceBar(MaxEndurance, Endurance);

            if (!isRecharging)
            {
                StartCoroutine(RechargeEndurance());
            }
        }
    }

    bool isRecharging = false;

    IEnumerator RechargeEndurance()
    {
        isRecharging = true;

        while (Endurance < MaxEndurance)
        {
            yield return new WaitForSeconds(1);

            if (IsOwner)
            {
                Endurance += 5;
                Endurance = Mathf.Min(Endurance, MaxEndurance);

                net_endurance.Value = Endurance;

                enduranceBar.UpdateEnduranceBar(MaxEndurance, Endurance);
            }
        }

        isRecharging = false;
    }

    public void CoinCollected(float amount)
    {
        Coins += amount;
        CoinText.text = Coins.ToString();

        playerInitialize.SavePlayerStats();
    }

    private void OnEnduranceChanged(float oldValue, float newValue)
    {
        // Update the endurance bar UI when the network variable changes
        enduranceBar.UpdateEnduranceBar(MaxEndurance, newValue);
    }
}
