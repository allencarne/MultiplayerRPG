using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorItem : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Color color; 

    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Item item;

    public void Init(PlayerStats _stats, Inventory _inventory, Item _item)
    {
        playerStats = _stats;
        inventory = _inventory;
        item = _item;

        UpdateUI();
        if (inventory != null)
        {
            inventory.OnCoinsChanged.AddListener(UpdateUI);
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnCoinsChanged.RemoveListener(UpdateUI);
        }
    }

    void UpdateUI()
    {
        if (playerStats == null) return;
        if (item == null) return;
        if (inventory == null) return;

        if (playerStats.Coins < item.Cost)
        {
            background.color = Color.gray;
            priceText.color = Color.red;
        }
        else
        {
            background.color = color;
            priceText.color = Color.black;
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
    }
}
