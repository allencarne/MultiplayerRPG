using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreatorUI : MonoBehaviour
{
    [SerializeField] CharacterCreator creator;
    [SerializeField] CharacterCustomizationData data;

    [SerializeField] GameObject bodyColorParent;
    [SerializeField] GameObject bodyColorPrefab;

    private void Start()
    {
        int skinIndex = 0;

        foreach (Color color in data.skinColors)
        {
            GameObject button = Instantiate(bodyColorPrefab, bodyColorParent.transform);
            Button buttonComponent = button.GetComponent<Button>();

            ColorBlock colors = buttonComponent.colors;
            colors.normalColor = color;
            buttonComponent.colors = colors;

            int currentIndex = skinIndex;
            buttonComponent.onClick.AddListener(() => SkinColorButton(currentIndex));

            skinIndex++;
        }
    }

    public void SkinColorButton(int index)
    {
        creator.skinColorIndex = index;
        creator.UpdateUI();
    }
}
