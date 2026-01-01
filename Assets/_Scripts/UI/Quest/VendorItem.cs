using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorItem : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI priceText;

    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Item item;

    public void UpdateUI()
    {
        if (playerStats == null) return;
        if (item == null) return;

        if (playerStats.Coins < item.Cost)
        {
            background.color = Color.gray;
            priceText.color = Color.red;
        }
    }

    public void AttemptToPurchase()
    {
        if (playerStats == null) return;
        if (inventory == null) return;
        if (item == null) return;

        if (playerStats.Coins >= item.Cost)
        {
            int avaliableSlots = inventory.GetFreeSlotCount();

            if (avaliableSlots > 0)
            {
                VendorInfoPanel panel = GetComponentInParent<VendorInfoPanel>();
                if (panel != null)
                {
                    panel.PurchaseAttempt(item);
                }
            }
            else
            {
                Debug.Log("Not enough inventory space to purchase Item!");
                return;
            }
        }

        UpdateUI();
    }
}
