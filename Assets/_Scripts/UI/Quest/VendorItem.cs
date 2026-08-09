using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Color color;

    [SerializeField] Image background;
    [SerializeField] Image image_QualityBorder;
    [SerializeField] Image image_ItemIcon;

    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public InventorySlotData slotData;

    public void Init(PlayerStats _stats, Inventory _inventory, InventorySlotData data)
    {
        playerStats = _stats;
        inventory = _inventory;
        slotData = data;

        UpdateUI();
        if (inventory != null)
        {
            inventory.OnCoinsChanged.AddListener(UpdateUI);
        }

        if (playerStats != null)
        {
            playerStats.PlayerLevel.OnValueChanged += OnPlayerLevelChanged;
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnCoinsChanged.RemoveListener(UpdateUI);
        }

        if (playerStats != null)
        {
            playerStats.PlayerLevel.OnValueChanged -= OnPlayerLevelChanged;
        }
    }

    void OnPlayerLevelChanged(int oldVal, int newVal)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerStats == null) return;
        if (slotData.item == null) return;
        if (inventory == null) return;

        if (playerStats.Coins < slotData.item.Cost)
        {
            background.color = Color.gray;
            priceText.color = Color.red;
        }
        else
        {
            background.color = color;
            priceText.color = Color.black;
        }

        image_ItemIcon.color = Color.white;
        image_ItemIcon.sprite = slotData.item.Icon;

        // Red Tint
        bool _isUnderLvl = IsUnderLevelRequirement(slotData.item);
        if (_isUnderLvl)
        {
            image_ItemIcon.color = Color.red;
        }

        // Set Quality Border
        image_QualityBorder.color = slotData.item.GetQualityColor(slotData.quality);
    }

    bool IsUnderLevelRequirement(Item item)
    {
        return item is Equipment equip && inventory.Stats.PlayerLevel.Value < equip.LevelRequirement;
    }

    public void AttemptToPurchase()
    {
        if (playerStats == null) return;
        if (inventory == null) return;
        if (slotData.item == null) return;

        if (playerStats.Coins >= slotData.item.Cost)
        {
            int avaliableSlots = inventory.GetFreeSlotCount();

            if (avaliableSlots > 0)
            {
                VendorInfoPanel panel = GetComponentInParent<VendorInfoPanel>();
                if (panel != null)
                {
                    panel.PurchaseAttempt(slotData);
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
