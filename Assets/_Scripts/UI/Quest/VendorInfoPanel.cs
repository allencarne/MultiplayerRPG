using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorInfoPanel : MonoBehaviour
{
    [SerializeField] GameObject Item_Prefab;
    [SerializeField] Transform parent;

    public void CreateItem(Item item)
    {
        GameObject itemUI = Instantiate(Item_Prefab, parent);

        Image image = itemUI.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = item.Icon;
        }

        TextMeshProUGUI nameText = itemUI.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = item.name;
        }
    }
}
