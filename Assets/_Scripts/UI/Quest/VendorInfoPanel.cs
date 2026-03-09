using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendorInfoPanel : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Inventory inventory;

    [SerializeField] GameObject Item_Prefab;
    [SerializeField] Transform parent;

    [SerializeField] GameObject ConfirmPurchasePanel;
    [SerializeField] GameObject ConfirmSellPanel;
    [SerializeField] GameObject yesPurchaseButton;
    [SerializeField] GameObject yesSellButton;
    Item itemToPurchase;

    InventorySlot fromSlot;
    int pendingSellSlotIndex;
    int pendingSellQuantity;
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
        if (ConfirmPurchasePanel.activeSelf || ConfirmSellPanel.activeSelf) return;

        itemToPurchase = item;
        ConfirmPurchasePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(yesPurchaseButton);
    }

    public void SellAttempt(InventorySlot _fromSlot, Item item)
    {
        if (ConfirmSellPanel.activeSelf || ConfirmPurchasePanel.activeSelf) return;

        fromSlot = _fromSlot;
        itemToSell = item;
        pendingSellSlotIndex = _fromSlot.slotIndex;
        pendingSellQuantity = _fromSlot.slotData.quantity;
        ConfirmSellPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(yesSellButton);
    }

    public void NoButton()
    {
        ConfirmPurchasePanel.SetActive(false);
        itemToPurchase = null;

        ConfirmSellPanel.SetActive(false);
        itemToSell = null;
        fromSlot = null;
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

        inventory.CoinCollected(itemToSell.SellValue * pendingSellQuantity);
        inventory.RemoveItemBySlot(pendingSellSlotIndex, pendingSellQuantity);
        itemToSell = null;
        fromSlot = null;
    }
}
