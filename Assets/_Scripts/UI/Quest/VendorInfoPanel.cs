using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorInfoPanel : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Inventory inventory;
    Item itemToPurchase;

    [SerializeField] GameObject Item_Prefab;
    [SerializeField] Transform parent;

    [SerializeField] GameObject ConfirmPanel;

    public void CreateItem(Item item)
    {
        GameObject itemUI = Instantiate(Item_Prefab, parent);

        VendorItem vendorItem = itemUI.GetComponent<VendorItem>();
        if (vendorItem != null)
        {
            vendorItem.item = item;
            vendorItem.inventory = inventory;
            vendorItem.playerStats = playerStats;
        }

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
            if (priceText != null) priceText.text = item.Cost.ToString();
        }
    }

    public void RemoveItems()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void PurchaseAttempt(Item item)
    {
        itemToPurchase = item;
        ConfirmPanel.SetActive(true);
    }

    public void NoButton()
    {
        ConfirmPanel.SetActive(false);
        itemToPurchase = null;
    }

    public void YesButton()
    {
        ConfirmPanel.SetActive(false);

        inventory.CoinSpent(itemToPurchase.Cost);
        inventory.AddItem(itemToPurchase);

        itemToPurchase = null;
    }
}
