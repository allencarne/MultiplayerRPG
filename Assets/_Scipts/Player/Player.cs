using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Components
    [SerializeField] CharacterCustomizationData customizationData;
    [SerializeField] Rigidbody2D rb;

    // Parts
    [SerializeField] SpriteRenderer body;
    [SerializeField] SpriteRenderer hair;

    // Stats
    public float moveSpeed;
    public int hairIndex;

    NetworkVariable<Color> _bodyColor = new(writePerm: NetworkVariableWritePermission.Owner);
    NetworkVariable<Color> _hairColor = new(writePerm: NetworkVariableWritePermission.Owner);

    private void Start()
    {
        if (IsOwner)
        {
        switch (PlayerPrefs.GetInt("SelectedCharacter"))
        {
            case 1:
                PlayerPrefs.GetString("Character1Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character1HairStyle");

                _bodyColor.Value = body.color;
                _hairColor.Value = hair.color;
                break;
            case 2:
                PlayerPrefs.GetString("Character2Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character2HairStyle");

                _bodyColor.Value = body.color;
                _hairColor.Value = hair.color;
                break;
            case 3:
                PlayerPrefs.GetString("Character3Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character3HairStyle");

                _bodyColor.Value = body.color;
                _hairColor.Value = hair.color;
                break;
        }
        }
        else
        {
            body.color = _bodyColor.Value;
            hair.color = _hairColor.Value;
        }
    }
}
