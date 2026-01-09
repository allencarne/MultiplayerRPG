
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorUI : MonoBehaviour
{
    [SerializeField] CharacterCreator creator;
    [SerializeField] CharacterCustomizationData data;

    [SerializeField] GameObject colorPreviewPrefab;
    [SerializeField] GameObject stylePreviewPrefab;

    [SerializeField] GameObject bodyColorParent;
    [SerializeField] GameObject hairColorParent;
    [SerializeField] GameObject eyeColorParent;
    [SerializeField] GameObject hairStyleParent;
    [SerializeField] GameObject eyeStyleParent;

    private List<GameObject> skinColorImages = new List<GameObject>();
    private List<GameObject> hairColorImages = new List<GameObject>();
    private List<GameObject> eyeColorImages = new List<GameObject>();
    private List<GameObject> hairStyleImages = new List<GameObject>();
    private List<GameObject> eyeStyleImages = new List<GameObject>();

    private void Start()
    {
        int skinColorIndex = 0;
        foreach (Color color in data.skinColors)
        {
            GameObject button = Instantiate(colorPreviewPrefab, bodyColorParent.transform);
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
            GameObject button = Instantiate(colorPreviewPrefab, hairColorParent.transform);
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
            GameObject button = Instantiate(colorPreviewPrefab, eyeColorParent.transform);
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

        int hairStyleIndex = 0;
        foreach (SpriteSet hair in data.hairs)
        {
            GameObject button = Instantiate(stylePreviewPrefab, hairStyleParent.transform);
            Button buttonComponent = button.GetComponent<Button>();

            int currentIndex = hairStyleIndex;

            GameObject hairImage = button.transform.Find("Style").gameObject;
            hairImage.GetComponent<Image>().sprite = hair.sprites[0];

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            hairStyleImages.Add(selectionImage);
            selectionImage.SetActive(false);

            buttonComponent.onClick.AddListener(() => HairStyleButton(currentIndex));

            hairStyleIndex++;
        }

        int eyeStyleIndex = 0;
        foreach (SpriteSet eye in data.eyes)
        {
            GameObject button = Instantiate(stylePreviewPrefab, eyeStyleParent.transform);
            Button buttonComponent = button.GetComponent<Button>();

            int currentIndex = eyeStyleIndex;

            GameObject hairImage = button.transform.Find("Style").gameObject;
            hairImage.GetComponent<Image>().sprite = eye.sprites[0];

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            eyeStyleImages.Add(selectionImage);
            selectionImage.SetActive(false);

            buttonComponent.onClick.AddListener(() => EyeStyleButton(currentIndex));

            eyeStyleIndex++;
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

    public void HairStyleButton(int index)
    {
        creator.hairStyleIndex = index;
        UpdateSelectionImages();
        creator.UpdateUI();
    }

    public void EyeStyleButton(int index)
    {
        creator.eyeStyleIndex = index;
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

        // Update hair style selection
        foreach (GameObject selectionImage in hairStyleImages)
        {
            selectionImage.SetActive(false);
        }
        if (hairStyleImages.Count > 0 && creator.hairStyleIndex < hairStyleImages.Count)
        {
            hairStyleImages[creator.hairStyleIndex].SetActive(true);
        }

        // Update eye style selection
        foreach (GameObject selectionImage in eyeStyleImages)
        {
            selectionImage.SetActive(false);
        }
        if (eyeStyleImages.Count > 0 && creator.eyeStyleIndex < eyeStyleImages.Count)
        {
            eyeStyleImages[creator.eyeStyleIndex].SetActive(true);
        }
    }
}
