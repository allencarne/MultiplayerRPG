using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerInitialize playerInitialize;
    public GameObject Spawn_Effect;
    [SerializeField] private CharacterCustomizationData customizationData;
    [SerializeField] private Rigidbody2D rb;
    public EnduranceBar enduranceBar;
    public Inventory inventory;

    [Header("UI")]
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] Canvas playerUI;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] RectTransform playerUIRect;

    [Header("Parts")]
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer hair;

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
    private NetworkVariable<FixedString32Bytes> net_playerName = new NetworkVariable<FixedString32Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> net_bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> net_hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

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
        Instantiate(Spawn_Effect, transform.position, transform.rotation);

        Endurance = MaxEndurance;

        // Set Endurance Bar UI
        enduranceBar.UpdateEnduranceBar(MaxEndurance, Endurance);
    }

    public override void OnNetworkSpawn()
    {
        // Attach callbacks for when the NetworkVariable values change
        net_playerName.OnValueChanged += OnNameChanged;
        net_bodyColor.OnValueChanged += OnBodyColorChanged;
        net_hairColor.OnValueChanged += OnHairColorChanged;
        net_endurance.OnValueChanged += OnEnduranceChanged;

        if (IsOwner)
        {
            InitializeOwnerCharacter();
            AssignPlayerCamera();

            // Initialize endurance network variable
            net_endurance.Value = Endurance;
        }
        else
        {
            // Set initial colors for non-owner players
            playerName.text = net_playerName.Value.ToString();
            body.color = net_bodyColor.Value;
            hair.color = net_hairColor.Value;
        }

        // Sync UI for non-owners
        enduranceBar.UpdateEnduranceBar(MaxEndurance, net_endurance.Value);
    }

    public override void OnDestroy()
    {
        // Unsubscribe from the callbacks to avoid memory leaks
        net_playerName.OnValueChanged -= OnNameChanged;
        net_bodyColor.OnValueChanged -= OnBodyColorChanged;
        net_hairColor.OnValueChanged -= OnHairColorChanged;
        net_endurance.OnValueChanged -= OnEnduranceChanged;

        base.OnDestroy();
    }

    void InitializeOwnerCharacter()
    {
        // Set initial values based on the selected character
        switch (PlayerPrefs.GetInt("SelectedCharacter"))
        {
            case 1:
                playerName.text = PlayerPrefs.GetString("Character1Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character1HairStyle");
                break;
            case 2:
                playerName.text = PlayerPrefs.GetString("Character2Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character2HairStyle");
                break;
            case 3:
                playerName.text = PlayerPrefs.GetString("Character3Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character3HairStyle");
                break;
        }

        // Update NetworkVariables
        net_playerName.Value = playerName.text;
        net_bodyColor.Value = body.color;
        net_hairColor.Value = hair.color;
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
        coinText.text = Coins.ToString();

        playerInitialize.SavePlayerStats();
    }

    void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerName.text = newName.ToString();
    }

    void OnBodyColorChanged(Color previousColor, Color newColor)
    {
        body.color = newColor;
    }

    void OnHairColorChanged(Color previousColor, Color newColor)
    {
        hair.color = newColor;
    }

    private void OnEnduranceChanged(float oldValue, float newValue)
    {
        // Update the endurance bar UI when the network variable changes
        enduranceBar.UpdateEnduranceBar(MaxEndurance, newValue);
    }
}
