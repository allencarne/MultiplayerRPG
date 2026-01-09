using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterCreator characterCreator;
    [SerializeField] GameObject CharacterCreatorPanel;
    [SerializeField] CharacterCustomizationData customizationData;

    [Header("Character 1")]
    [SerializeField] TextMeshProUGUI Character1NameText;
    [SerializeField] Image head1;
    [SerializeField] Image body1;
    [SerializeField] Image hair1;
    [SerializeField] Image eye1;
    [SerializeField] Button deleteButton1;
    [SerializeField] GameObject statPreview1;

    [Header("Character 2")]
    [SerializeField] TextMeshProUGUI Character2NameText;
    [SerializeField] Image head2;
    [SerializeField] Image body2;
    [SerializeField] Image hair2;
    [SerializeField] Image eye2;
    [SerializeField] Button deleteButton2;
    [SerializeField] GameObject statPreview2;

    [Header("Character 3")]
    [SerializeField] TextMeshProUGUI Character3NameText;
    [SerializeField] Image head3;
    [SerializeField] Image body3;
    [SerializeField] Image hair3;
    [SerializeField] Image eye3;
    [SerializeField] Button deleteButton3;
    [SerializeField] GameObject statPreview3;

    private void Start()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (!character1Exists && !character2Exists && !character3Exists)
        {
            CharacterCreatorPanel.SetActive(true);
        }

        deleteButton1.gameObject.SetActive(false);
        statPreview1.SetActive(false);
        deleteButton2.gameObject.SetActive(false);
        statPreview2.SetActive(false);
        deleteButton3.gameObject.SetActive(false);
        statPreview3.SetActive(false);

        LoadCharacters();
    }

    public void LoadCharacters()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (character1Exists)
        {
            Character1NameText.text = PlayerPrefs.GetString("Character1Name");
            head1.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
            body1.color = customizationData.skinColors[PlayerPrefs.GetInt("Character1SkinColor")];
            hair1.sprite = customizationData.hairs[PlayerPrefs.GetInt("Character1HairStyle")].sprites[0];
            hair1.color = customizationData.hairColors[PlayerPrefs.GetInt("Character1HairColor")];
            eye1.sprite = customizationData.eyes[PlayerPrefs.GetInt("Character1EyeStyle")].sprites[0];

            Material eyeMat = eye1.material;
            eyeMat.SetColor("_NewColor", customizationData.eyeColors[PlayerPrefs.GetInt("Character1EyeColor")]);

            deleteButton1.gameObject.SetActive(true);
            statPreview1.SetActive(true);
        }
        if (character2Exists)
        {
            Character2NameText.text = PlayerPrefs.GetString("Character2Name");
            head2.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
            head2.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
            body2.color = customizationData.skinColors[PlayerPrefs.GetInt("Character2SkinColor")];
            hair2.sprite = customizationData.hairs[PlayerPrefs.GetInt("Character2HairStyle")].sprites[0];
            hair2.color = customizationData.hairColors[PlayerPrefs.GetInt("Character2HairColor")];
            eye2.sprite = customizationData.eyes[PlayerPrefs.GetInt("Character2EyeStyle")].sprites[0];

            Material eyeMat = eye2.material;
            eyeMat.SetColor("_NewColor", customizationData.eyeColors[PlayerPrefs.GetInt("Character2EyeColor")]);

            deleteButton2.gameObject.SetActive(true);
            statPreview2.SetActive(true);
        }
        if (character3Exists)
        {
            Character3NameText.text = PlayerPrefs.GetString("Character3Name");
            head3.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
            head3.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
            body3.color = customizationData.skinColors[PlayerPrefs.GetInt("Character3SkinColor")];
            hair3.sprite = customizationData.hairs[PlayerPrefs.GetInt("Character3HairStyle")].sprites[0];
            hair3.color = customizationData.hairColors[PlayerPrefs.GetInt("Character3HairColor")];
            eye3.sprite = customizationData.eyes[PlayerPrefs.GetInt("Character3EyeStyle")].sprites[0];

            Material eyeMat = eye3.material;
            eyeMat.SetColor("_NewColor", customizationData.eyeColors[PlayerPrefs.GetInt("Character3EyeColor")]);

            deleteButton3.gameObject.SetActive(true);
            statPreview3.SetActive(true);
        }
    }

    public void DeleteCharacter1()
    {
        PlayerPrefs.SetString("Character1Name", "Empty");
        PlayerPrefs.SetInt("Character1SkinColor", 0);
        PlayerPrefs.SetInt("Character1HairStyle", 0);
        PlayerPrefs.SetInt("Character1HairColor", 0);
        PlayerPrefs.SetInt("Character1EyeStyle", 0);
        PlayerPrefs.SetInt("Character1EyeColor", 0);
        PlayerPrefs.SetInt("Character1", 0);

        ResetCharacterSaveData(1);

        Character1NameText.text = "Empty";
        head1.color = Color.white;
        body1.color = Color.white;
        hair1.sprite = customizationData.hairs[0].sprites[0];
        hair1.color = Color.black;
        eye1.sprite = customizationData.eyes[0].sprites[0];

        Material eyeMat = eye1.material;
        eyeMat.SetColor("_NewColor", Color.black);
    }

    public void DeleteCharacter2()
    {
        PlayerPrefs.SetString("Character2Name", "Empty");
        PlayerPrefs.SetInt("Character2SkinColor", 0);
        PlayerPrefs.SetInt("Character2HairStyle", 0);
        PlayerPrefs.SetInt("Character2HairColor", 0);
        PlayerPrefs.SetInt("Character2EyeStyle", 0);
        PlayerPrefs.SetInt("Character2EyeColor", 0);
        PlayerPrefs.SetInt("Character2", 0);

        ResetCharacterSaveData(2);

        Character2NameText.text = "Empty";
        head2.color = Color.white;
        body2.color = Color.white;
        hair2.sprite = customizationData.hairs[0].sprites[0];
        hair2.color = Color.black;
        eye2.sprite = customizationData.eyes[0].sprites[0];

        Material eyeMat = eye2.material;
        eyeMat.SetColor("_NewColor", Color.black);
    }

    public void DeleteCharacter3()
    {
        PlayerPrefs.SetString("Character3Name", "Empty");
        PlayerPrefs.SetInt("Character3SkinColor", 0);
        PlayerPrefs.SetInt("Character3HairStyle", 0);
        PlayerPrefs.SetInt("Character3HairColor", 0);
        PlayerPrefs.SetInt("Character3EyeStyle", 0);
        PlayerPrefs.SetInt("Character3EyeColor", 0);
        PlayerPrefs.SetInt("Character3", 0);

        ResetCharacterSaveData(3);

        Character3NameText.text = "Empty";
        head3.color = Color.white;
        body3.color = Color.white;
        hair3.sprite = customizationData.hairs[0].sprites[0];
        hair3.color = Color.black;
        eye3.sprite = customizationData.eyes[0].sprites[0];

        Material eyeMat = eye3.material;
        eyeMat.SetColor("_NewColor", Color.black);
    }

    public void SelectCharacter1()
    {
        bool character1Exists = PlayerPrefs.GetInt("Character1") == 1;

        if (character1Exists)
        {
            PlayerPrefs.SetInt("SelectedCharacter", 1);

            SceneManager.LoadScene("Beach");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }

        deleteButton1.gameObject.SetActive(false);
        statPreview1.SetActive(false);
    }

    public void SelectCharacter2()
    {
        bool character2Exists = PlayerPrefs.GetInt("Character2") == 1;

        if (character2Exists)
        {
            PlayerPrefs.SetInt("SelectedCharacter", 2);

            SceneManager.LoadScene("Beach");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }

        deleteButton2.gameObject.SetActive(false);
        statPreview2.SetActive(false);
    }

    public void SelectCharacter3()
    {
        bool character3Exists = PlayerPrefs.GetInt("Character3") == 1;

        if (character3Exists)
        {
            PlayerPrefs.SetInt("SelectedCharacter", 3);

            SceneManager.LoadScene("Beach");
        }
        else
        {
            characterCreator.ResetCreatorUI();
            CharacterCreatorPanel.SetActive(true);
        }

        deleteButton3.gameObject.SetActive(false);
        statPreview3.SetActive(false);
    }

    private void ResetCharacterSaveData(int slot)
    {
        string[] keys = new string[]
        {
        $"{slot}PlayerLevel",
        $"{slot}CurrentExperience",
        $"{slot}Coins",
        $"{slot}AP",
        $"{slot}MaxHealth",
        $"{slot}MaxFury",
        $"{slot}MaxEndurance",
        $"{slot}EnduranceRecharge",
        $"{slot}Speed",
        $"{slot}Damage",
        $"{slot}AttackSpeed",
        $"{slot}CDR",
        $"{slot}BaseArmor",
        $"{slot}FirstPassive",
        $"{slot}SecondPassive",
        $"{slot}ThirdPassive",
        $"{slot}Basic",
        $"{slot}Offensive",
        $"{slot}Mobility",
        $"{slot}Defensive",
        $"{slot}Utility",
        $"{slot}Ultimate",
        };

        foreach (string key in keys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        for (int i = 0; i < 30; i++)
        {
            PlayerPrefs.DeleteKey($"Character{slot}_InventorySlot_{i}");
        }

        for (int i = 0; i < 8; i++)
        {
            PlayerPrefs.DeleteKey($"Character{slot}_EquipmentSlot_{i}");
        }
    }
}