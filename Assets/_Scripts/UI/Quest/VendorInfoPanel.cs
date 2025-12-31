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

        Transform iconTransform = itemUI.transform.Find("Icon");
        if (iconTransform != null)
        {
            Image icon = iconTransform.GetComponent<Image>();
            if (icon != null)
            {
                icon.color = Color.white;
                icon.sprite = item.Icon;
            }
        }

        Transform nameTransform = itemUI.transform.Find("Text_Name");
        if (nameTransform != null)
        {
            TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
            if (nameText != null) nameText.text = item.name;
        }

        Transform priceTransform = itemUI.transform.Find("Text_Price");
        if (priceTransform != null)
        {
            TextMeshProUGUI priceText = priceTransform.GetComponent<TextMeshProUGUI>();
            if (priceText != null) priceText.text = item.SellValue.ToString();
        }

    }
}
