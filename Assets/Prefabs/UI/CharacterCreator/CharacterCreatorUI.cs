
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorUI : MonoBehaviour
{
    [SerializeField] CharacterCreator creator;
    [SerializeField] CharacterCustomizationData data;

    [SerializeField] GameObject bodyColorParent;
    [SerializeField] GameObject bodyColorPrefab;

    [SerializeField] GameObject hairColorParent;
    [SerializeField] GameObject hairColorPrefab;

    [SerializeField] GameObject eyeColorParent;
    [SerializeField] GameObject eyeColorPrefab;

    private List<GameObject> skinColorImages = new List<GameObject>();
    private List<GameObject> hairColorImages = new List<GameObject>();
    private List<GameObject> eyeColorImages = new List<GameObject>();

    private void Start()
    {
        int skinColorIndex = 0;
        foreach (Color color in data.skinColors)
        {
            GameObject button = Instantiate(bodyColorPrefab, bodyColorParent.transform);
            Button buttonComponent = button.GetComponent<Button>();
            Image imageComponent = button.GetComponent<Image>();

            imageComponent.color = color;

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            skinColorImages.Add(selectionImage);
            selectionImage.SetActive(false);

            int currentIndex = skinColorIndex;
            buttonComponent.onClick.AddListener(() => SkinColorButton(currentIndex));

            skinColorIndex++;
        }

        int hairColorIndex = 0;
        foreach (Color color in data.hairColors)
        {
            GameObject button = Instantiate(hairColorPrefab, hairColorParent.transform);
            Button buttonComponent = button.GetComponent<Button>();
            Image imageComponent = button.GetComponent<Image>();

            imageComponent.color = color;

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            hairColorImages.Add(selectionImage);
            selectionImage.SetActive(false);

            int currentIndex = hairColorIndex;
            buttonComponent.onClick.AddListener(() => HairColorButton(currentIndex));

            hairColorIndex++;
        }

        int eyeColorIndex = 0;
        foreach (Color color in data.eyeColors)
        {
            GameObject button = Instantiate(eyeColorPrefab, eyeColorParent.transform);
            Button buttonComponent = button.GetComponent<Button>();
            Image imageComponent = button.GetComponent<Image>();

            imageComponent.color = color;

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            eyeColorImages.Add(selectionImage);
            selectionImage.SetActive(false);

            int currentIndex = eyeColorIndex;
            buttonComponent.onClick.AddListener(() => EyeColorButton(currentIndex));

            eyeColorIndex++;
        }

        UpdateSelectionImages();
    }

    public void SkinColorButton(int index)
    {
        creator.skinColorIndex = index;
        UpdateSelectionImages();
        creator.UpdateUI();
    }

    public void HairColorButton(int index)
    {
        creator.hairColorIndex = index;
        UpdateSelectionImages();
        creator.UpdateUI();
    }

    public void EyeColorButton(int index)
    {
        creator.eyeColorIndex = index;
        UpdateSelectionImages();
        creator.UpdateUI();
    }

    public void UpdateSelectionImages()
    {
        // Update skin color selection
        foreach (GameObject selectionImage in skinColorImages)
        {
            selectionImage.SetActive(false);
        }
        if (skinColorImages.Count > 0 && creator.skinColorIndex < skinColorImages.Count)
        {
            skinColorImages[creator.skinColorIndex].SetActive(true);
        }

        // Update hair color selection
        foreach (GameObject selectionImage in hairColorImages)
        {
            selectionImage.SetActive(false);
        }
        if (hairColorImages.Count > 0 && creator.hairColorIndex < hairColorImages.Count)
        {
            hairColorImages[creator.hairColorIndex].SetActive(true);
        }

        // Update eye color selection
        foreach (GameObject selectionImage in eyeColorImages)
        {
            selectionImage.SetActive(false);
        }
        if (eyeColorImages.Count > 0 && creator.eyeColorIndex < eyeColorImages.Count)
        {
            eyeColorImages[creator.eyeColorIndex].SetActive(true);
        }
    }
}
