using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorUI : MonoBehaviour
{
    [SerializeField] CharacterCreator creator;
    [SerializeField] CharacterCustomizationData data;

    [SerializeField] GameObject bodyColorParent;
    [SerializeField] GameObject bodyColorPrefab;

    private List<GameObject> skinColorSelectionImages = new List<GameObject>();

    private void Start()
    {
        int skinIndex = 0;

        foreach (Color color in data.skinColors)
        {
            GameObject button = Instantiate(bodyColorPrefab, bodyColorParent.transform);
            Button buttonComponent = button.GetComponent<Button>();
            Image imageComponent = button.GetComponent<Image>();

            imageComponent.color = color;

            GameObject selectionImage = button.transform.Find("Image").gameObject;
            skinColorSelectionImages.Add(selectionImage);
            selectionImage.SetActive(false);

            int currentIndex = skinIndex;
            buttonComponent.onClick.AddListener(() => SkinColorButton(currentIndex));

            skinIndex++;
        }

        if (skinColorSelectionImages.Count > 0)
        {
            skinColorSelectionImages[0].SetActive(true);
        }
    }

    public void SkinColorButton(int index)
    {
        creator.skinColorIndex = index;

        foreach (GameObject selectionImage in skinColorSelectionImages)
        {
            selectionImage.SetActive(false);
        }

        skinColorSelectionImages[index].SetActive(true);

        creator.UpdateUI();
    }
}
