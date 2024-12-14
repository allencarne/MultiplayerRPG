using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Player : NetworkBehaviour
{
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] Canvas playerUI;

    // Components
    [SerializeField] private CharacterCustomizationData customizationData;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] EnduranceBar enduranceBar;

    // Parts
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer hair;

    // Stats
    public float moveSpeed;
    public float endurance;
    public float maxEndurance;
    public int hairIndex;

    private NetworkVariable<Color> _bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> _hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

    private void Start()
    {
        endurance = maxEndurance;

        enduranceBar.UpdateEnduranceBar(maxEndurance, endurance);
    }

    public override void OnNetworkSpawn()
    {
        // Attach callbacks for when the NetworkVariable values change
        _bodyColor.OnValueChanged += OnBodyColorChanged;
        _hairColor.OnValueChanged += OnHairColorChanged;

        if (IsOwner)
        {
            InitializeOwnerCharacter();
            AssignPlayerCamera();
        }
        else
        {
            // Set initial colors for non-owner players
            body.color = _bodyColor.Value;
            hair.color = _hairColor.Value;
        }
    }

    private void InitializeOwnerCharacter()
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
        _bodyColor.Value = body.color;
        _hairColor.Value = hair.color;
    }

    private void AssignPlayerCamera()
    {
        // Assign the camera to follow this player
        if (Camera.main.GetComponent<CameraFollow>().playerTransform == null)
        {
            Camera.main.GetComponent<CameraFollow>().playerTransform = transform;
        }
        else
        {
            GameObject cameraInstance = Instantiate(cameraPrefab);
            CameraFollow cameraFollow = cameraInstance.GetComponent<CameraFollow>();
            cameraFollow.playerTransform = transform;
        }
    }

    private void OnBodyColorChanged(Color previousColor, Color newColor)
    {
        body.color = newColor;
    }

    private void OnHairColorChanged(Color previousColor, Color newColor)
    {
        hair.color = newColor;
    }

    public override void OnDestroy()
    {
        // Unsubscribe from the callbacks to avoid memory leaks
        _bodyColor.OnValueChanged -= OnBodyColorChanged;
        _hairColor.OnValueChanged -= OnHairColorChanged;

        // Call the base class OnDestroy to ensure proper behavior
        base.OnDestroy();
    }

    public void UpdateEndurance(float amount)
    {
        endurance -= amount;
        enduranceBar.UpdateEnduranceBar(maxEndurance, endurance);

        if (!isRecharging)
        {
            StartCoroutine(RechargeEndurance());
        }
    }

    private bool isRecharging = false;

    IEnumerator RechargeEndurance()
    {
        isRecharging = true;

        while (endurance < maxEndurance)
        {
            yield return new WaitForSeconds(1);
            endurance += 5;
            endurance = Mathf.Min(endurance, maxEndurance);
            enduranceBar.UpdateEnduranceBar(maxEndurance, endurance);
        }

        isRecharging = false;
    }
}
