using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorInfoPanel : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Inventory inventory;

    [SerializeField] GameObject Item_Prefab;
    [SerializeField] Transform parent;

    [SerializeField] GameObject ConfirmPurchasePanel;
    [SerializeField] GameObject ConfirmSellPanel;
    Item itemToPurchase;

    InventorySlot fromSlot;
    Item itemToSell;

    private void OnDisable()
    {
        NoButton();
    }

    public void CreateItem(Item item)
    {
        GameObject itemUI = Instantiate(Item_Prefab, parent);
        itemUI.transform.localPosition = Vector3.zero;

        VendorItem vendorItem = itemUI.GetComponent<VendorItem>();
        if (vendorItem != null)
        {
            vendorItem.Init(playerStats,inventory,item);
        }

        VendorItemToolTip toolTip = itemUI.GetComponentInChildren<VendorItemToolTip>();
        if (toolTip != null)
        {
            toolTip.Init(item);
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
            if (priceText != null) priceText.text = item.Cost.ToString()+ " <sprite index=0>";
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
        if (ConfirmPurchasePanel.activeSelf) return;

        itemToPurchase = item;
        ConfirmPurchasePanel.SetActive(true);
    }

    public void SellAttempt(InventorySlot _fromSlot, Item item)
    {
        if (ConfirmSellPanel.activeSelf) return;

        fromSlot = _fromSlot;
        itemToSell = item;
        ConfirmSellPanel.SetActive(true);
    }

    public void NoButton()
    {
        ConfirmPurchasePanel.SetActive(false);
        itemToPurchase = null;

        ConfirmSellPanel.SetActive(false);
        itemToSell = null;
    }

    public void YesPurchaseButton()
    {
        ConfirmPurchasePanel.SetActive(false);

        inventory.CoinSpent(itemToPurchase.Cost);
        inventory.AddItem(itemToPurchase);

        itemToPurchase = null;
    }

    public void YesSellButton()
    {
        ConfirmSellPanel.SetActive(false);

        InventorySlotData data = fromSlot.slotData;
        inventory.CoinCollected(data.item.SellValue * data.quantity);
        inventory.RemoveItemBySlot(fromSlot.slotIndex, data.quantity);

        itemToSell = null;
    }
}
