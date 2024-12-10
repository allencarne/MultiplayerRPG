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

    // Variables
    public int hairIndex;

    private void Start()
    {
        switch (PlayerPrefs.GetInt("SelectedCharacter"))
        {
            case 1:
                Debug.Log("Selected Character 1");

                PlayerPrefs.GetString("Character1Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
                //hair.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character1HairStyle")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character1HairStyle");
                break;
            case 2:
                Debug.Log("Selected Character 2");

                PlayerPrefs.GetString("Character2Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
                //hair.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character2HairStyle")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character2HairStyle");
                break;
            case 3:
                Debug.Log("Selected Character 3");

                PlayerPrefs.GetString("Character3Name");
                body.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
                //hair.sprite = customizationData.hairStyles[PlayerPrefs.GetInt("Character3HairStyle")];
                hair.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
                hairIndex = PlayerPrefs.GetInt("Character3HairStyle");
                break;
        }
    }
}
