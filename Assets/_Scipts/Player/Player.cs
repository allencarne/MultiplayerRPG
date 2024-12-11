using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] GameObject cameraPrefab;

    // Components
    [SerializeField] private CharacterCustomizationData customizationData;
    [SerializeField] private Rigidbody2D rb;

    // Parts
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private SpriteRenderer hair;

    // Stats
    public float moveSpeed;
    public int hairIndex;

    private NetworkVariable<Color> _bodyColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Color> _hairColor = new NetworkVariable<Color>(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        // Attach callbacks for when the NetworkVariable values change
        _bodyColor.OnValueChanged += OnBodyColorChanged;
        _hairColor.OnValueChanged += OnHairColorChanged;

        if (IsOwner)
        {
            // Set initial values based on the selected character
            switch (PlayerPrefs.GetInt("SelectedCharacter"))
            {
                case 1:
                    body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
                    hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
                    hairIndex = PlayerPrefs.GetInt("Character1HairStyle");
                    break;
                case 2:
                    body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
                    hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
                    hairIndex = PlayerPrefs.GetInt("Character2HairStyle");
                    break;
                case 3:
                    body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
                    hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
                    hairIndex = PlayerPrefs.GetInt("Character3HairStyle");
                    break;
            }

            // Update NetworkVariables
            _bodyColor.Value = body.color;
            _hairColor.Value = hair.color;

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
        else
        {
            // Set initial colors for non-owner players
            body.color = _bodyColor.Value;
            hair.color = _hairColor.Value;
        }
    }

    private void OnBodyColorChanged(Color previousColor, Color newColor)
    {
        body.color = newColor; // Apply the new body color
    }

    private void OnHairColorChanged(Color previousColor, Color newColor)
    {
        hair.color = newColor; // Apply the new hair color
    }

    public override void OnDestroy()
    {
        // Unsubscribe from the callbacks to avoid memory leaks
        _bodyColor.OnValueChanged -= OnBodyColorChanged;
        _hairColor.OnValueChanged -= OnHairColorChanged;

        // Call the base class OnDestroy to ensure proper behavior
        base.OnDestroy();
    }
}
